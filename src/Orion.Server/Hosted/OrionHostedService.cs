using Orion.Core.Irc.Server.Data.Internal;
using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Events.Server;
using Orion.Core.Server.Hosted;
using Orion.Core.Server.Interfaces.Services.Base;
using Orion.Core.Server.Interfaces.Services.System;
using Orion.Core.Server.Internal;
using Orion.Core.Server.Internal.Services;
using Orion.Irc.Core.Data.Internal;

namespace Orion.Server.Hosted;

public class OrionHostedService : BaseOrionHostedService
{
    private readonly List<IrcListenerDefinition> _ircCommandListeners;


    public OrionHostedService(
        ILogger<OrionHostedService> logger, List<ServiceDefinitionObject> serviceDefinitions,
        IEventBusService eventBusService, IServiceProvider serviceProvider, List<IrcListenerDefinition> ircCommandListeners
    ) : base(logger, serviceDefinitions, eventBusService, serviceProvider)
    {
        _ircCommandListeners = ircCommandListeners;
    }


    protected override async Task BeforeStartAsync()
    {
        var serverContext = ServiceProvider.GetRequiredService<IrcServerContextData>();
        var textService = ServiceProvider.GetRequiredService<ITextTemplateService>();


        textService.AddVariable("network_name", serverContext.NetworkName);
        textService.AddVariable("server_name", serverContext.ServerName);
        textService.AddVariable("server_started", serverContext.ServerStartTime);
    }

    protected override async Task OnReady()
    {
        foreach (var listenerType in _ircCommandListeners)
        {
            ServiceProvider.GetRequiredService(listenerType.Type);
        }
    }
}
