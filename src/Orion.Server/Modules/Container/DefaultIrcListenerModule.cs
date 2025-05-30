using Orion.Core.Irc.Server.Extensions;
using Orion.Core.Server.Extensions;
using Orion.Core.Server.Interfaces.Modules;
using Orion.Server.Handlers;

namespace Orion.Server.Modules.Container;

public class DefaultIrcListenerModule : IOrionContainerModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        return services
            .AddIrcListener<ConnectionHandler>()
            .AddIrcListener<PingPongHandler>()
            .AddIrcListener<WelcomeHandler>()
            .AddIrcListener<UserModeHandler>()
            .AddIrcListener<UserPrivMessageHandler>()
            .AddIrcListener<ChannelsHandler>()
            .AddIrcListener<NoticeHandler>()
            .AddIrcListener<AwayHandler>()
            .AddIrcListener<OperHandler>()
            .AddIrcListener<WhoHandler>()
            .AddIrcListener<ServerHandler>()
            .AddIrcListener<UserHandler>()

            ;
    }
}
