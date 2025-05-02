using Microsoft.Extensions.DependencyInjection;
using Orion.Core.Server.Data.Config.Internal;
using Orion.Core.Server.Data.Config.Sections;
using Orion.Core.Server.Interfaces.Services.System;
using Orion.Core.Server.Services;

namespace Orion.Core.Server.Extensions;

public static class OrionServicesExtensions
{
    /// <summary>
    ///  Adds the diagnostic service to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static IServiceCollection AddDiagnosticService(
        this IServiceCollection services, DiagnosticServiceConfig? config = null
    )
    {
        config ??= new DiagnosticServiceConfig();
        services.AddSingleton(config);
        return services.AddService<IDiagnosticService, DiagnosticService>(priority: -1);
    }

    /// <summary>
    ///  Adds the event bus system service to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static IServiceCollection AddEventBusService(this IServiceCollection services, EventBusConfig? config = null)
    {
        config ??= new EventBusConfig();
        services.AddSingleton(config);
        return services.AddService<IEventBusService, EventBusService>();
    }

    /// <summary>
    ///  Adds the ProcessQueueService engine system service to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static IServiceCollection AddProcessQueueService(
        this IServiceCollection services, ProcessQueueConfig? config = null
    )
    {
        config ??= new ProcessQueueConfig();
        services.AddSingleton(config);
        return services.AddService<IProcessQueueService, ProcessQueueService>();
    }

    public static IServiceCollection AddScriptEngineService(
        this IServiceCollection services, ScriptEngineConfig? config = null
    )
    {
        config ??= new ScriptEngineConfig();
        services.AddSingleton(config);
        return services.AddService<IScriptEngineService, ScriptEngineService>();
    }
}
