
using Orion.Core.Irc.Server.Interfaces.Services.Irc;
using Orion.Core.Irc.Server.Interfaces.Services.System;
using Orion.Core.Server.Data.Config.Sections;
using Orion.Core.Server.Extensions;
using Orion.Core.Server.Interfaces.Modules;
using Orion.Core.Server.Interfaces.Services.System;

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
        services.AddSingleton(new EventBusConfig()
        {
            MaxConcurrentTasks = 4
        });


        services.AddService<IAuthService, AuthService>();

        services
            .AddService<IIrcCommandService, IrcCommandService>()
            .AddService<IIrcSessionService, IrcSessionService>()
            .AddService<IChannelManagerService, ChannelManagerService>()
            .AddService<IIrcCommandParser, IrcCommandParser>();

        return services
            .AddService<IEventBusService, EventBusService>()
            .AddService<IDiagnosticService, DiagnosticService>()
            .AddService<ITextTemplateService, TextTemplateService>()
            .AddService<ISchedulerSystemService, SchedulerSystemService>()
            .AddService<IEventDispatcherService, EventDispatcherService>()
            .AddService<IScriptEngineService, ScriptEngineService>()
            .AddService<INetworkService, NetworkService>()
            .AddService<IVersionService, VersionService>()
            .AddService<IProcessQueueService, ProcessQueueService>();

    }
}
