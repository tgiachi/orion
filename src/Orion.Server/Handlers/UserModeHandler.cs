using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Data.Sessions;
using Orion.Core.Server.Handlers.Base;
using Orion.Core.Server.Interfaces.Listeners.Commands;
using Orion.Foundations.Types;
using Orion.Irc.Core.Commands;
using Orion.Irc.Core.Commands.Errors;
using Orion.Irc.Core.Types;

namespace Orion.Server.Handlers;

public class UserModeHandler : BaseIrcCommandListener, IIrcCommandHandler<ModeCommand>
{
    public UserModeHandler(ILogger<UserModeHandler> logger, IrcCommandListenerContext context) : base(logger, context)
    {
        RegisterCommandHandler<ModeCommand>(this, ServerNetworkType.Clients);
    }

    public async Task OnCommandReceivedAsync(
        IrcUserSession session, ServerNetworkType serverNetworkType, ModeCommand command
    )
    {
        if (command.TargetType == ModeTargetType.Channel)
        {
            return;
        }


        if (!command.Target.Equals(session.NickName, StringComparison.InvariantCultureIgnoreCase))
        {
            await session.SendCommandAsync(
                ErrUsersDontMatch.Create(
                    ServerHostName,
                    command.Target
                )
            );
            return;
        }

        if (ListenerContext.SessionService.FindByNickName(command.Target) == null)
        {
            await session.SendCommandAsync(ErrNoSuchNick.Create(ServerHostName, command.Target, session.NickName));
        }

        
    }
}
