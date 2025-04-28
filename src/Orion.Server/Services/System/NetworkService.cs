using System.Security.Authentication;
using NetCoreServer;
using Orion.Core.Irc.Server.Interfaces.Services.System;
using Orion.Core.Server.Data.Config;
using Orion.Core.Server.Events.Server;
using Orion.Core.Server.Extensions;
using Orion.Core.Server.Interfaces.Services.System;
using Orion.Core.Server.Listeners.EventBus;
using Orion.Foundations.Extensions;
using Orion.Foundations.Types;
using Orion.Network.Core.Interfaces.Services;
using Orion.Network.Tcp.Servers;

namespace Orion.Server.Services.System;

public class NetworkService : INetworkService, IEventBusListener<ServerReadyEvent>
{
    private readonly ILogger _logger;

    private readonly OrionServerConfig _orionConfig;
    private readonly INetworkTransportManager _networkTransportManager;

    private SslContext _sslContext;

    private readonly IEventBusService _eventBusService;

    public NetworkService(
        ILogger<NetworkService> logger, INetworkTransportManager networkTransportManager,
        IEventBusService eventBusService, OrionServerConfig orionConfig
    )
    {
        _logger = logger;
        _networkTransportManager = networkTransportManager;
        _eventBusService = eventBusService;
        _orionConfig = orionConfig;

        _eventBusService.Subscribe(this);

        _networkTransportManager.ClientConnected += ClientConnected;
        _networkTransportManager.ClientDisconnected += ClientDisconnected;
    }

    private void ClientDisconnected(string transportId, string sessionId, string endpoint)
    {
        _logger.LogInformation(
            "Client disconnected: {TransportId} - {SessionId} - {Endpoint}",
            transportId,
            sessionId,
            endpoint
        );
    }

    private void ClientConnected(string transportId, string sessionId, string endpoint)
    {
        _logger.LogInformation(
            "Client connected: {TransportId} - {SessionId} - {Endpoint}",
            transportId,
            sessionId,
            endpoint
        );
    }

    public async Task HandleAsync(ServerReadyEvent @event, CancellationToken cancellationToken = default)
    {
        await StartNetworkAsync();
    }

    private async Task StartNetworkAsync()
    {
        _logger.LogInformation("Starting network...");

        _logger.LogDebug("Preloading SSL context");

        if (!string.IsNullOrWhiteSpace(_orionConfig.Network.Certificate.CertificatePath))
        {
            var certificate = _orionConfig.Network.Certificate.LoadCertificate();

            _sslContext = new SslContext(SslProtocols.Tls13, certificate);

            _logger.LogDebug("SSL context loaded");
        }

        foreach (var bind in _orionConfig.Network.Binds)
        {
            foreach (var port in bind.Ports.ToPorts())
            {
                _logger.LogDebug(
                    "Starting server type {Type} on port {Port} - Network for: {NetType}",
                    bind.Secure ? "SSL" : "Unsecure",
                    port,
                    bind.NetworkType
                );

                if (bind.Secure)
                {
                    AddSecureTcpServer(port, bind.Host, bind.NetworkType);
                }
                else
                {
                    AddNonSecureTcpServer(port, bind.Host, bind.NetworkType);
                }
            }
        }


        await _networkTransportManager.StartAsync();
    }

    private void AddNonSecureTcpServer(int port, string ipAddress, ServerNetworkType networkType)
    {
        var server = new NonSecureTcpServer(Guid.NewGuid().ToString(), networkType, ipAddress.ToIpAddress(), port);

        _logger.LogInformation("Starting server on (non-SSL) TCP {IpAddress}:{Port}", ipAddress, port);
        _networkTransportManager.AddTransport(server);
    }

    private void AddSecureTcpServer(int port, string ipAddress, ServerNetworkType networkType)
    {
        if (_sslContext == null)
        {
            throw new InvalidOperationException("SSL context is not initialized");
        }

        var server = new SecureTcpServer(Guid.NewGuid().ToString(), networkType, _sslContext, ipAddress.ToIpAddress(), port);
        _logger.LogInformation("Starting server on (SSL) TCP {IpAddress}:{Port}", ipAddress, port);
        _networkTransportManager.AddTransport(server);
    }
}
