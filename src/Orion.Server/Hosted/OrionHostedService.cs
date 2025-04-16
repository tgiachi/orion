using HyperCube.Postman.Interfaces.Services;
using Orion.Core.Server.Events.Server;
using Orion.Core.Server.Interfaces.Services.Base;
using Orion.Core.Server.Internal;

namespace Orion.Server.Hosted;

public class OrionHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly List<ServiceDefinitionObject> _serviceDefinitions;
    private readonly IHyperPostmanService _hyperPostmanService;
    private readonly ILogger _logger;


    public OrionHostedService(
        ILogger<OrionHostedService> logger, List<ServiceDefinitionObject> serviceDefinitions,
        IHyperPostmanService hyperPostmanService, IServiceProvider serviceProvider
    )
    {
        _serviceDefinitions = serviceDefinitions;
        _hyperPostmanService = hyperPostmanService;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var serviceDef in _serviceDefinitions)
        {
            try
            {
                var service = _serviceProvider.GetService(serviceDef.ServiceType);
                if (service == null)
                {
                    _logger.LogError("Service {ServiceName} not registered", serviceDef.ServiceType.Name);
                    throw new InvalidOperationException($"Service {serviceDef.ServiceType.Name} not registered");
                }

                _logger.LogDebug("Starting service {ServiceName}", serviceDef.ServiceType.Name);

                if (service is IOrionStartService orionService)
                {
                    await orionService.StartAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting service {ServiceName}", serviceDef.ServiceType.Name);
                throw;
            }
        }

        await _hyperPostmanService.PublishAsync(new ServerStartedEvent(), cancellationToken);
        await _hyperPostmanService.PublishAsync(new ServerReadyEvent(), cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _hyperPostmanService.PublishAsync(new ServerStoppingEvent(), cancellationToken);

        foreach (var serviceDef in _serviceDefinitions)
        {
            try
            {
                var service = _serviceProvider.GetService(serviceDef.ServiceType);
                if (service == null)
                {
                    _logger.LogError("Service {ServiceName} not registered", serviceDef.ServiceType.Name);
                    continue;
                }

                _logger.LogDebug("Stopping service {ServiceName}", serviceDef.ServiceType.Name);


                if (service is IOrionStartService orionService)
                {
                    await orionService.StopAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping service {ServiceName}", serviceDef.ServiceType.Name);
            }
        }
    }
}
