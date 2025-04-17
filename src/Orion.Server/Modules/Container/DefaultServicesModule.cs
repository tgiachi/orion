using HyperCube.Postman.Config;
using HyperCube.Postman.Interfaces.Services;
using HyperCube.Postman.Services;
using Orion.Core.Server.Extensions;
using Orion.Core.Server.Interfaces.Modules;
using Orion.Core.Server.Interfaces.Services;
using Orion.Irc.Core.Interfaces.Parser;
using Orion.Irc.Core.Services;
using Orion.Server.Services;
using Orion.Server.Services.Irc;
using Orion.Server.Services.System;

namespace Orion.Server.Modules.Container;

public class DefaultServicesModule : IOrionContainerModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        services.AddSingleton(new HyperPostmanConfig()
        {
            MaxConcurrentTasks = 4
        });

        services
            .AddService<IIrcCommandService, IrcCommandService>()
            .AddService<IIrcCommandParser, IrcCommandParser>();

        return services
            .AddService<IHyperPostmanService, HyperPostmanService>()
            .AddService<IDiagnosticService, DiagnosticService>()
            .AddService<ITextTemplateService, TextTemplateService>()
            .AddService<ISchedulerSystemService, SchedulerSystemService>()
            .AddService<IEventDispatcherService, EventDispatcherService>()
            .AddService<IScriptEngineService, ScriptEngineService>()
            .AddService<INetworkService, NetworkService>()
            .AddService<IProcessQueueService, ProcessQueueService>();

    }
}
