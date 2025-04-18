using HyperCube.Postman.Interfaces.Services;
using Orion.Core.Server.Data.Config;
using Orion.Core.Server.Data.Options;
using Orion.Core.Server.Interfaces.Services.Irc;

namespace Orion.Core.Server.Data.Internal;

public class IrcCommandListenerContext
{
    public IIrcCommandService CommandService { get; }
    public IHyperPostmanService PostmanService { get; }
    public IIrcSessionService SessionService { get; }
    public AppContextData<OrionServerOptions, OrionServerConfig> AppContext { get; }

    public IrcServerContextData ServerContextData { get; set; } = new();


    public IrcCommandListenerContext(
        IIrcCommandService commandService,
        IHyperPostmanService postmanService,
        IIrcSessionService sessionService,
        IrcServerContextData serverContextData,
        AppContextData<OrionServerOptions, OrionServerConfig> appContext)
    {
        ServerContextData = serverContextData;
        CommandService = commandService;
        PostmanService = postmanService;
        SessionService = sessionService;
        AppContext = appContext;
    }
}
