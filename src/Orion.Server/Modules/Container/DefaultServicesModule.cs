using Orion.Core.Server.Extensions;
using Orion.Core.Server.Interfaces.Modules;
using Orion.Core.Server.Interfaces.Services;
using Orion.Server.Services;

namespace Orion.Server.Modules.Container;

public class DefaultServicesModule : IOrionContainerModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        return services
            .AddService<IDiagnosticService, DiagnosticService>()
            .AddService<ITextTemplateService, TextTemplateService>()
            .AddService<ISchedulerSystemService, SchedulerSystemService>()
            .AddService<IEventDispatcherService, EventDispatcherService>()
            .AddService<IScriptEngineService, ScriptEngineService>()
            .AddService<IProcessQueueService, ProcessQueueService>();

    }
}
