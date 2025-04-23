using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Data.Sessions;
using Orion.Core.Server.Events.Irc.Opers;
using Orion.Core.Server.Handlers.Base;
using Orion.Core.Server.Interfaces.Listeners.Commands;
using Orion.Core.Server.Interfaces.Services.Irc;
using Orion.Foundations.Types;
using Orion.Foundations.Utils;
using Orion.Irc.Core.Commands;
using Orion.Irc.Core.Commands.Errors;
using Orion.Irc.Core.Commands.Replies;

namespace Orion.Server.Handlers;

public class OperHandler : BaseIrcCommandListener, IIrcCommandHandler<OperCommand>, IIrcCommandHandler<KillCommand>
{

    private readonly IChannelManagerService _channelManagerService;
    public OperHandler(ILogger<OperHandler> logger, IrcCommandListenerContext context, IChannelManagerService channelManagerService) : base(logger, context)
    {
        _channelManagerService = channelManagerService;
        RegisterCommandHandler<OperCommand>(this, ServerNetworkType.Clients);
        RegisterCommandHandler<KillCommand>(this, ServerNetworkType.Clients);
    }

    public async Task OnCommandReceivedAsync(
        IrcUserSession session, ServerNetworkType serverNetworkType, OperCommand command
    )
    {
        var exist = Config.Irc.Opers.Entries.FirstOrDefault(s => s.NickName == command.Username);


        if (exist == null || !exist.IsPasswordValid(command.Password) || !HostMaskUtils.IsHostMaskMatch(exist.Host, session.FullAddress))
        {
            await session.SendCommandAsync(
                ErrNoOperHost.Create(
                    ServerHostName,
                    session.NickName
                )
            );

            await PublishEventAsync(new OperWrongPasswordEvent(session.SessionId));

            return;
        }

        session.SetOperator(true);

        if (!string.IsNullOrEmpty(exist.VHost))
        {
            session.VHostName = exist.VHost;
        }

        await session.SendCommandAsync(
            RplYoureOper.Create(
                ServerHostName,
                session.NickName
            )
        );

        await PublishEventAsync(new OperLoggedInEvent(session.NickName));
    }

    public async Task OnCommandReceivedAsync(
        IrcUserSession session, ServerNetworkType serverNetworkType, KillCommand command
    )
    {
        if (!session.IsOperator)
        {
            await session.SendCommandAsync(
                ErrNoOperHost.Create(
                    ServerHostName,
                    session.NickName
                )
            );

            return;
        }

        var targetSession = GetSessionByNickName(command.TargetNickname);

        if (targetSession == null)
        {
            await session.SendCommandAsync(
                ErrNoSuchNick.Create(
                    ServerHostName,
                    session.NickName,
                    command.TargetNickname
                )
            );

            return;
        }

        if (targetSession.IsOperator)
        {
            await session.SendCommandAsync(
                ErrCantKillServer.Create(
                    ServerHostName,
                    session.NickName,
                    command.TargetNickname
                )
            );

            return;
        }


        await session.SendCommandAsync(
            KillCommand.CreateWithSource(session.NickName, command.TargetNickname, command.Reason)
        );

        targetSession.SendCommandAsync(
            KillCommand.CreateWithSource(session.NickName, command.TargetNickname, command.Reason)
        );

        var usersConnected = await _channelManagerService.GetConnectedUsersAsync(command.TargetNickname);

        foreach (var user in usersConnected)
        {
            var userSession = GetSessionByNickName(user);
            await userSession.SendCommandAsync(
                KillCommand.CreateWithSource(session.NickName, command.TargetNickname, command.Reason)
            );
        }


        await targetSession.DisconnectAsync();

    }
}
