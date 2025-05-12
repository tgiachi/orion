using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orion.Core.Server.Events.Server;
using Orion.Core.Server.Interfaces.Services.Base;
using Orion.Core.Server.Interfaces.Services.System;
using Orion.Core.Server.Internal.Services;

namespace Orion.Core.Server.Hosted;

public abstract class BaseOrionHostedService : IHostedService
{
    private readonly List<ServiceDefinitionObject> _serviceDefinitions;

    protected IServiceProvider ServiceProvider { get; }

    protected IEventBusService EventBusService { get; }

    protected ILogger Logger { get; }

    protected BaseOrionHostedService(
        ILogger<BaseOrionHostedService> logger, List<ServiceDefinitionObject> serviceDefinitions,
        IEventBusService eventBusService, IServiceProvider serviceProvider
    )
    {
        _serviceDefinitions = serviceDefinitions;
        EventBusService = eventBusService;
        ServiceProvider = serviceProvider;
        Logger = logger;
    }


    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await BeforeStartAsync();

        foreach (var serviceDef in _serviceDefinitions.DistinctBy(s => s.ServiceType)
                     .OrderBy(serviceDef => serviceDef.Priority))
        {
            try
            {
                var service = ServiceProvider.GetService(serviceDef.ServiceType);
                if (service == null)
                {
                    Logger.LogError("Service {ServiceName} not registered", serviceDef.ServiceType.Name);
                    throw new InvalidOperationException($"Service {serviceDef.ServiceType.Name} not registered");
                }

                Logger.LogDebug(
                    "Starting service {ServiceName} [Priority: {Priority}]",
                    serviceDef.ServiceType.Name,
                    serviceDef.Priority
                );

                if (service is IOrionStartService orionService)
                {
                    await orionService.StartAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error starting service {ServiceName}", serviceDef.ServiceType.Name);
                throw;
            }
        }

        await EventBusService.PublishAsync(new ServerStartedEvent(), cancellationToken);
        await EventBusService.PublishAsync(new ServerReadyEvent(), cancellationToken);

        await OnReady();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await EventBusService.PublishAsync(new ServerStoppingEvent(), cancellationToken);

        await OnStopping();

        foreach (var serviceDef in _serviceDefinitions.DistinctBy(s => s.ServiceType)
                     .OrderByDescending(serviceDef => serviceDef.Priority))
        {
            try
            {
                var service = ServiceProvider.GetService(serviceDef.ServiceType);
                if (service == null)
                {
                    Logger.LogError("Service {ServiceName} not registered", serviceDef.ServiceType.Name);
                    continue;
                }

                Logger.LogDebug("Stopping service {ServiceName}", serviceDef.ServiceType.Name);


                if (service is IOrionStartService orionService)
                {
                    await orionService.StopAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error stopping service {ServiceName}", serviceDef.ServiceType.Name);
            }
        }
    }

    protected virtual async Task OnReady()
    {
    }

    protected virtual async Task OnStopping()
    {
    }

    protected virtual async Task BeforeStartAsync()
    {
    }
}
