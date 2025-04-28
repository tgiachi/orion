using Orion.Core.Irc.Server.Data.Internal;
using Orion.Core.Irc.Server.Data.Sessions;
using Orion.Core.Irc.Server.Handlers.Base;
using Orion.Core.Irc.Server.Interfaces.Listeners.Commands;
using Orion.Core.Server.Data.Internal;
using Orion.Foundations.Types;
using Orion.Irc.Core.Commands;
using Orion.Irc.Core.Commands.Errors;
using Orion.Irc.Core.Data.Messages;

namespace Orion.Server.Handlers;

public class NoticeHandler : BaseIrcCommandListener, IIrcCommandHandler<NoticeCommand>
{
    public NoticeHandler(ILogger<NoticeHandler> logger, IrcCommandListenerContext context) : base(logger, context)
    {
        RegisterCommandHandler(this, ServerNetworkType.Clients);
    }

    public async Task OnCommandReceivedAsync(
        IrcUserSession session, ServerNetworkType serverNetworkType, NoticeCommand command
    )
    {
        if (command.TargetType == PrivMessageTarget.TargetType.User)
        {
            var targetSession = GetSessionByNickName(command.Target);

            if (targetSession != null)
            {
                await targetSession.SendCommandAsync(command);

                return;
            }

            await session.SendCommandAsync(
                ErrNoSuchNick.Create(
                    ServerHostName,
                    session.NickName,
                    command.Target
                )
            );
        }
    }
}
