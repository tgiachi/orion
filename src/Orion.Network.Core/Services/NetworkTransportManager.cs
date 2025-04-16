using System.Buffers;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Orion.Network.Core.Data;
using Orion.Network.Core.Interfaces.Services;
using Orion.Network.Core.Interfaces.Transports;
using Orion.Network.Core.Parsers;

namespace Orion.Network.Core.Services;

public class NetworkTransportManager : INetworkTransportManager
{
    private readonly ILogger _logger;

    private readonly Subject<NetworkMetricData> _networkMetricsSubject = new();

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private readonly Task _outputTask;

    private readonly ConcurrentDictionary<string, string> _sessionsTransports = new();

    private readonly ConcurrentDictionary<string, NetworkMetricData> _sessionsMetrics = new();

    private readonly IDisposable _metricsSubscription;

    public event INetworkTransport.ClientConnectedHandler? ClientConnected;

    public event INetworkTransport.ClientDisconnectedHandler? ClientDisconnected;

    public List<NetworkTransportData> Transports { get; } = new();

    public IObservable<NetworkMetricData> NetworkMetrics => _networkMetricsSubject;

    public Channel<NetworkMessageData> IncomingMessages { get; }

    public Channel<NetworkMessageData> OutgoingMessages { get; }

    public NetworkTransportManager(ILogger<NetworkTransportManager> logger)
    {
        _logger = logger;

        IncomingMessages = Channel.CreateUnbounded<NetworkMessageData>();
        OutgoingMessages = Channel.CreateUnbounded<NetworkMessageData>();

        _outputTask = Task.Run(OutputTaskAction);

        _metricsSubscription = Observable.Interval(TimeSpan.FromSeconds(60)).Subscribe(_ => EmitMetrics());
    }

    private void EmitMetrics()
    {
        if (!_sessionsMetrics.IsEmpty)
        {
            foreach (var (sessionId, metrics) in _sessionsMetrics)
            {
                _logger.LogDebug("Session {SessionId} metrics: {Metrics}", sessionId, metrics);

                _networkMetricsSubject.OnNext(metrics);
            }
        }
    }

    private async Task OutputTaskAction()
    {
        while (_cancellationTokenSource.IsCancellationRequested == false)
        {
            await foreach (var message in OutgoingMessages.Reader.ReadAllAsync(_cancellationTokenSource.Token))
            {
                try
                {
                    var sessionTransportId = _sessionsTransports.GetValueOrDefault(message.SessionId);

                    if (sessionTransportId == null)
                    {
                        _logger.LogWarning("Session transport not found for session {sessionId}", message.SessionId);
                        continue;
                    }

                    var transport = Transports.FirstOrDefault(t => t.Id == sessionTransportId);

                    var byteArray = ArrayPool<byte>.Shared.Rent(message.Message.Length);
                    var bytes = Encoding.UTF8.GetBytes(message.Message, byteArray);

                    _sessionsMetrics[message.SessionId].AddBytesOut(bytes);
                    _sessionsMetrics[message.SessionId].AddPacketsOut();

                    await transport.Transport.SendAsync(message.SessionId, byteArray);

                    _logger.LogTrace(
                        "Sent message to session {SessionId} on transport {TransportName}: {Message}",
                        message.SessionId,
                        sessionTransportId,
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
            _logger.LogDebug(
                "Starting transport {TransportName}:{Port} - {Id}",
                transport.Name,
                transport.Port,
                transport.Id
            );
            await transport.Transport.StartAsync();
            _logger.LogDebug(
                "Started transport {TransportName}:{Port} - {Id}",
                transport.Name,
                transport.Port,
                transport.Id
            );
        }
    }

    private void TransportOnMessageReceived(string transportId, string sessionId, ReadOnlyMemory<byte> data)
    {
        var messages = NewLineMessageParser.FastParseMessages(data);

        foreach (var message in messages)
        {
            _logger.LogTrace(
                "{TransportName} - SessionId: {Session} IpClient {Endpoint} received message: {Message}",
                sessionId,
                transportId,
                sessionId,
                message
            );

            _sessionsMetrics[sessionId].AddBytesIn(data.Length);
            _sessionsMetrics[sessionId].AddPacketsIn();

            var messageData = new NetworkMessageData(sessionId, message);
            IncomingMessages.Writer.TryWrite(messageData);
        }
    }

    private void TransportOnClientDisconnected(string transportId, string sessionId, string endpoint)
    {
        _logger.LogDebug(
            "{TransportName} - SessionId: {Session} IpClient {Endpoint} disconnected",
            sessionId,
            transportId,
            endpoint
        );
        _sessionsTransports.TryRemove(sessionId, out _);
        _sessionsMetrics.TryRemove(sessionId, out _);

        ClientDisconnected?.Invoke(transportId, sessionId, endpoint);
    }

    private void TransportOnClientConnected(string transportId, string sessionId, string endpoint)
    {
        _logger.LogDebug(
            "{TransportName} - SessionId: {Session} IpClient {Endpoint} connected",
            sessionId,
            transportId,
            endpoint
        );
        _sessionsTransports.TryAdd(sessionId, transportId);
        _sessionsMetrics.TryAdd(sessionId, new NetworkMetricData());

        ClientConnected?.Invoke(transportId, sessionId, endpoint);
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        foreach (var transport in Transports)
        {
            _logger.LogDebug("Starting transport {TransportName}", transport.Name);
            await transport.Transport.StopAsync();
        }
    }

    public async Task EnqueueMessageAsync(NetworkMessageData messageData, CancellationToken cancellationToken = default)
    {
        await OutgoingMessages.Writer.WriteAsync(messageData, cancellationToken);
    }

    public void AddTransport(INetworkTransport transport)
    {
        if (Transports.Any(t => t.Name == transport.Name && t.Port == transport.Port))
        {
            throw new InvalidOperationException($"Transport with name {transport.Name} already exists.");
        }

        transport.ClientConnected += TransportOnClientConnected;
        transport.ClientDisconnected += TransportOnClientDisconnected;
        transport.MessageReceived += TransportOnMessageReceived;

        Transports.Add(new NetworkTransportData(transport));
    }

    public void Dispose()
    {
        _cancellationTokenSource.Dispose();
        _metricsSubscription.Dispose();
        GC.SuppressFinalize(this);
    }
}
