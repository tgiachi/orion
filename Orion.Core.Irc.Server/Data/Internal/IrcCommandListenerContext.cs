using Orion.Core.Irc.Server.Interfaces.Services.Irc;
using Orion.Core.Server.Data.Config;
using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Data.Options;
using Orion.Core.Server.Interfaces.Services.System;

namespace Orion.Core.Irc.Server.Data.Internal;

public class IrcCommandListenerContext
{
    public IIrcCommandService CommandService { get; }
    public IEventBusService EventBusService { get; }
    public IIrcSessionService SessionService { get; }
    public AppContextData<OrionServerOptions, OrionServerConfig> AppContext { get; }


    public ITextTemplateService TextTemplateService { get; }

    public IrcServerContextData ServerContextData { get; set; } = new();


    public IrcCommandListenerContext(
        IIrcCommandService commandService,
        IEventBusService eventBusService,
        IIrcSessionService sessionService,
        IrcServerContextData serverContextData,
        ITextTemplateService textTemplateService,
        AppContextData<OrionServerOptions,
            OrionServerConfig> appContext)
    {
        ServerContextData = serverContextData;
        CommandService = commandService;
        EventBusService = eventBusService;
        SessionService = sessionService;
        AppContext = appContext;
        TextTemplateService = textTemplateService;
    }
}
