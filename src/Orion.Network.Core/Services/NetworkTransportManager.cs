using System.Buffers;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Orion.Network.Core.Data;
using Orion.Network.Core.Interfaces.Services;
using Orion.Network.Core.Interfaces.Transports;

namespace Orion.Network.Core.Services;

public class NetworkTransportManager : INetworkTransportManager
{
    private readonly ILogger _logger;

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private readonly Task _outputTask;

    private readonly ConcurrentDictionary<string, string> _sessionsTransports = new();

    private readonly ConcurrentDictionary<string, NetworkMetricData> _sessionsMetrics = new();

    public List<INetworkTransport> Transports { get; } = new();

    public Channel<NetworkMessageData> IncomingMessages { get; }

    public Channel<NetworkMessageData> OutgoingMessages { get; }

    public NetworkTransportManager(ILogger<NetworkTransportManager> logger)
    {
        _logger = logger;

        IncomingMessages = Channel.CreateUnbounded<NetworkMessageData>();
        OutgoingMessages = Channel.CreateUnbounded<NetworkMessageData>();

        _outputTask = Task.Run(OutputTaskAction);
    }

    private async Task OutputTaskAction()
    {
        while (_cancellationTokenSource.IsCancellationRequested == false)
        {
            await foreach (var message in OutgoingMessages.Reader.ReadAllAsync(_cancellationTokenSource.Token))
            {
                try
                {
                    var sessionTransport = _sessionsTransports.GetValueOrDefault(message.SessionId);

                    if (sessionTransport == null)
                    {
                        _logger.LogWarning("Session transport not found for session {sessionId}", message.SessionId);
                        continue;
                    }

                    var transport = Transports.FirstOrDefault(t => t.Name == sessionTransport);

                    var byteArray = ArrayPool<byte>.Shared.Rent(message.Message.Length);
                    var bytes = Encoding.UTF8.GetBytes(message.Message, byteArray);

                    _sessionsMetrics[message.SessionId].AddBytesOut(bytes);
                    _sessionsMetrics[message.SessionId].AddPacketsOut();

                    await transport.SendAsync(message.SessionId, byteArray);

                    _logger.LogTrace(
                        "Sent message to session {SessionId} on transport {TransportName}: {Message}",
                        message.SessionId,
                        sessionTransport,
                        message.Message
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error while processing outgoing message for session {SessionId}: {Message}",
                        message.SessionId,
                        message.Message
                    );
                }
                finally
                {
                    OutgoingMessages.Writer.TryComplete();
                }
            }
        }
    }


    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        foreach (var transport in Transports)
        {
            _logger.LogDebug("Starting transport {TransportName}", transport.Name);

            transport.ClientConnected += TransportOnClientConnected;
            transport.ClientDisconnected += TransportOnClientDisconnected;

            await transport.StartAsync();
        }
    }

    private void TransportOnClientDisconnected(string transportName, string sessionId, string endpoint)
    {
        _logger.LogDebug(
            "{TransportName} - SessionId: {Session} IpClient {Endpoint} disconnected",
            sessionId,
            transportName,
            endpoint
        );
        _sessionsTransports.TryRemove(sessionId, out _);
        _sessionsMetrics.TryRemove(sessionId, out _);
    }

    private void TransportOnClientConnected(string transportName, string sessionId, string endpoint)
    {
        _logger.LogDebug(
            "{TransportName} - SessionId: {Session} IpClient {Endpoint} connected",
            sessionId,
            transportName,
            endpoint
        );
        _sessionsTransports.TryAdd(sessionId, transportName);
        _sessionsMetrics.TryAdd(sessionId, new NetworkMetricData());
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        foreach (var transport in Transports)
        {
            _logger.LogDebug("Starting transport {TransportName}", transport.Name);
            await transport.StopAsync();
        }
    }

    public async Task EnqueueMessageAsync(NetworkMessageData messageData, CancellationToken cancellationToken = default)
    {
        await OutgoingMessages.Writer.WriteAsync(messageData, cancellationToken);
    }

    public void AddTransport(INetworkTransport transport)
    {
        if (Transports.Any(t => t.Name == transport.Name))
        {
            throw new InvalidOperationException($"Transport with name {transport.Name} already exists.");
        }

        Transports.Add(transport);
    }

    public void Dispose()
    {
        _cancellationTokenSource.Dispose();
        _outputTask.Dispose();
        GC.SuppressFinalize(this);
    }
}
