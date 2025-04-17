using Orion.Core.Server.Extensions;
using Orion.Core.Server.Interfaces.Modules;
using Orion.Irc.Core.Commands;

namespace Orion.Server.Modules.Container;

public class IrcCommandModule : IOrionContainerModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        return services
                .AddIrcCommand<NickCommand>()
                .AddIrcCommand<UserCommand>()
            ;
    }
}
