using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Data.Sessions;
using Orion.Core.Server.Handlers.Base;
using Orion.Core.Server.Interfaces.Listeners.Commands;
using Orion.Foundations.Types;
using Orion.Irc.Core.Commands;
using Orion.Irc.Core.Commands.Errors;

namespace Orion.Server.Handlers;

public class UserPrivMessageHandler : BaseIrcCommandListener, IIrcCommandHandler<PrivMsgCommand>
{
    public UserPrivMessageHandler(ILogger<UserPrivMessageHandler> logger, IrcCommandListenerContext context) : base(
        logger,
        context
    )
    {
        RegisterCommandHandler<PrivMsgCommand>(this, ServerNetworkType.Clients);
    }

    public async Task OnCommandReceivedAsync(
        IrcUserSession session, ServerNetworkType serverNetworkType, PrivMsgCommand command
    )
    {
        if (command.IsChannelMessage)
        {
            return;
        }

        var targetSession = ListenerContext.SessionService.FindByNickName(command.Target);

        if (targetSession == null)
        {
            await session.SendCommandAsync(
                ErrNoSuchNick.Create(
                    ServerHostName,
                    session.NickName,
                    command.Target
                )
            );
            return;
        }

        await targetSession.SendCommandAsync(
            PrivMsgCommand.CreateFromUser(session.FullAddress, command.Target, command.Message)
        );
    }
}
