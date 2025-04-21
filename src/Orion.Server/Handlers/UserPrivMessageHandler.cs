using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Data.Sessions;
using Orion.Core.Server.Events.Irc;
using Orion.Core.Server.Handlers.Base;
using Orion.Core.Server.Interfaces.Listeners.Commands;
using Orion.Foundations.Types;
using Orion.Irc.Core.Commands;
using Orion.Irc.Core.Commands.Errors;
using Orion.Irc.Core.Commands.Replies;
using Orion.Irc.Core.Data.Messages;

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
        if (command.Targets.Length == 0)
        {
            await session.SendCommandAsync(
                ErrNoRecipients.Create(
                    ServerHostName,
                    session.NickName,
                    command.Code
                )
            );
            return;
        }

        if (command.Targets.Length > Config.Irc.Limits.MaxTargets)
        {
            await session.SendCommandAsync(
                ErrTooManyTargets.Create(
                    ServerHostName,
                    session.NickName,
                    command.Target
                )
            );
            return;
        }

        var targets = command.Targets.Where(s => s.IsUser).ToList();

        if (!targets.Any())
        {
            return;
        }

        if (string.IsNullOrEmpty(command.Message))
        {
            await session.SendCommandAsync(
                ErrNoTextToSend.Create(
                    ServerHostName,
                    session.NickName
                )
            );
            return;
        }

        foreach (var target in targets)
        {
            await SendPrivMessage(session, target, command.Message);
        }
    }

    private async Task SendPrivMessage(IrcUserSession session, string target, string message)
    {
        var targetSession = ListenerContext.SessionService.FindByNickName(target);

        if (targetSession == null)
        {
            await session.SendCommandAsync(
                ErrNoSuchNick.Create(
                    ServerHostName,
                    session.NickName,
                    target
                )
            );

            return;
        }

        await targetSession.SendCommandAsync(
            PrivMsgCommand.CreateFromUser(session.FullAddress, target, message)
        );

        await PublishEventAsync(
            new UserPrivateMessageEvent(session.FullAddress, target, message, PrivMessageTarget.TargetType.User)
        );

        if (targetSession.IsAway)
        {
            await session.SendCommandAsync(
                RplAway.Create(
                    ServerHostName,
                    session.NickName,
                    targetSession.NickName,
                    targetSession.AwayMessage
                )
            );
        }
    }
}
