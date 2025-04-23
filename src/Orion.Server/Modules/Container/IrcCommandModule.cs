using System.Net.NetworkInformation;
using Orion.Core.Server.Extensions;
using Orion.Core.Server.Interfaces.Modules;
using Orion.Irc.Core.Commands;
using Orion.Server.Handlers;

namespace Orion.Server.Modules.Container;

public class IrcCommandModule : IOrionContainerModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        return services
                .AddIrcCommand<NickCommand>()
                .AddIrcCommand<UserCommand>()
                .AddIrcCommand<CapCommand>()
                .AddIrcCommand<PingCommand>()
                .AddIrcCommand<PongCommand>()
                .AddIrcCommand<PassCommand>()
                .AddIrcCommand<PrivMsgCommand>()
                .AddIrcCommand<JoinCommand>()
                .AddIrcCommand<PartCommand>()
                .AddIrcCommand<TopicCommand>()
                .AddIrcCommand<QuitCommand>()
                .AddIrcCommand<ModeCommand>()
                .AddIrcCommand<AwayCommand>()
                .AddIrcCommand<OperCommand>()
                .AddIrcCommand<KillCommand>()
                .AddIrcCommand<NamesCommand>()
                .AddIrcCommand<ListCommand>()
                .AddIrcCommand<WhoCommand>()
                .AddIrcCommand<WhoIsCommand>()
                .AddIrcCommand<IsonCommand>()
                .AddIrcCommand<NoticeCommand>()
            ;
    }
}
