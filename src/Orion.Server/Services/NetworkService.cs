using System.Security.Authentication;
using HyperCube.Postman.Interfaces.Services;
using NetCoreServer;
using Orion.Core.Extensions;
using Orion.Core.Server.Data.Config;
using Orion.Core.Server.Events.Server;
using Orion.Core.Server.Extensions;
using Orion.Core.Server.Interfaces.Services;
using Orion.Network.Core.Interfaces.Services;

namespace Orion.Server.Services;

public class NetworkService : INetworkService, ILetterListener<ServerReadyEvent>
{
    private readonly ILogger _logger;

    private readonly OrionServerConfig _orionConfig;
    private readonly INetworkTransportManager _networkTransportManager;

    private SslContext _sslContext;

    private readonly IHyperPostmanService _hyperPostmanService;

    public NetworkService(
        ILogger<NetworkService> logger, INetworkTransportManager networkTransportManager,
        IHyperPostmanService hyperPostmanService, OrionServerConfig orionConfig
    )
    {
        _logger = logger;
        _networkTransportManager = networkTransportManager;
        _hyperPostmanService = hyperPostmanService;
        _orionConfig = orionConfig;

        _hyperPostmanService.Subscribe(this);
    }

    public async Task HandleAsync(ServerReadyEvent @event, CancellationToken cancellationToken = default)
    {
        await StartNetworkAsync();
    }

    private async Task StartNetworkAsync()
    {
        _logger.LogInformation("Starting network...");

        _logger.LogDebug("Preloading SSL context");

        if (!string.IsNullOrWhiteSpace(_orionConfig.Network.SSL.CertificatePath))
        {
            var certificate = _orionConfig.Network.SSL.LoadCertificate();

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
            }
        }
    }
}
