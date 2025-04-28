using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Orion.Network.Core.Data;
using Orion.Network.Core.Extensions;
using Orion.Network.Core.Interfaces.Services;
using Orion.Network.Core.Interfaces.Transports;
using Orion.Network.Core.Parsers;

namespace Orion.Network.Core.Services;

public class NetworkTransportManager : INetworkTransportManager
{
    private readonly ILogger _logger;

    private readonly Subject<NetworkMetricData> _networkMetricsSubject = new();

    private readonly CancellationTokenSource _cancellationTokenSource = new();

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

        _ = Task.Run(OutputTaskAction);

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
            await foreach (var message in OutgoingMessages.Reader.ReadAllAsync())
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


                    _sessionsMetrics[message.SessionId].AddBytesOut(message.Message.Length);
                    _sessionsMetrics[message.SessionId].AddPacketsOut();

                    await transport.Transport.SendAsync(message.SessionId, message.Message);

                    var messageString = Encoding.UTF8.GetString(message.Message);
                    var sanitizedMessage = messageString.Replace(Environment.NewLine, " ");

                    _logger.LogDebug(
                        "-> {IpEndpoint}- {SessionId} - {Type} - {Message}",
                        _sessionsMetrics[message.SessionId].Endpoint,
                        message.SessionId.ToShortSessionId(),
                        message.ServerNetworkType,
                        sanitizedMessage
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
            }
        }
    }


    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        foreach (var transport in Transports)
        {
            _logger.LogDebug(
                "Starting transport {TransportName}({Security}):{Port} - {Id}",
                transport.Transport.Protocol,
                transport.Transport.Security,
                transport.Port,
                transport.Id.ToShortSessionId()
            );
            await transport.Transport.StartAsync();
            _logger.LogDebug(
                "Started transport {TransportName}({Security}):{Port} - {Id}",
                transport.Transport.Protocol,
                transport.Transport.Security,
                transport.Port,
                transport.Id.ToShortSessionId()
            );
        }
    }

    private void TransportOnMessageReceived(string transportId, string sessionId, ReadOnlyMemory<byte> data)
    {

        var messageString = Encoding.UTF8.GetString(data.Span).Replace(Environment.NewLine, " ");

        _sessionsMetrics[sessionId].AddBytesIn(data.Length);
        _sessionsMetrics[sessionId].AddPacketsIn();

        var transport = Transports.FirstOrDefault(t => t.Id == transportId);

        _logger.LogDebug(
            "<- {Endpoint} - {SessionId} - {Type} - {Message}",
            _sessionsMetrics[sessionId].Endpoint,
            sessionId.ToShortSessionId(),
            transport.ServerNetworkType,
            messageString
        );

  ;

        var messageData = new NetworkMessageData(sessionId, data.ToArray(), transport.ServerNetworkType);
        IncomingMessages.Writer.TryWrite(messageData);
        //}
    }

    private void TransportOnClientDisconnected(string transportId, string sessionId, string endpoint)
    {
        _logger.LogDebug(
            "{TransportName} - SessionId: {Session} IpClient {Endpoint} disconnected",
            sessionId.ToShortSessionId(),
            transportId.ToShortSessionId(),
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
            sessionId.ToShortSessionId(),
            transportId.ToShortSessionId(),
            endpoint
        );
        _sessionsTransports.TryAdd(sessionId, transportId);
        _sessionsMetrics.TryAdd(sessionId, new NetworkMetricData(sessionId, endpoint));


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

    public async Task EnqueueMessageAsync(NetworkMessageData messageData)
    {
        await OutgoingMessages.Writer.WriteAsync(messageData);
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

    public NetworkTransportData GetTransport(string Id)
    {
        var transport = Transports.FirstOrDefault(t => t.Id == Id);

        if (transport == null)
        {
            throw new InvalidOperationException($"Transport with id {Id} not found.");
        }

        return transport;
    }

    public Task DisconnectAsync(string sessionId)
    {
        if (_sessionsTransports.TryGetValue(sessionId, out var transportId))
        {
            var transport = Transports.FirstOrDefault(t => t.Id == transportId);

            _logger.LogDebug(
                "Disconnecting session {SessionId} from transport {TransportName}",
                sessionId.ToShortSessionId(),
                transport.Name
            );
            return transport.Transport.DisconnectAsync(sessionId);
        }

        throw new InvalidOperationException($"Session {sessionId} not found.");
    }

    public void Dispose()
    {
        _cancellationTokenSource.Dispose();
        _metricsSubscription.Dispose();
        GC.SuppressFinalize(this);
    }
}
