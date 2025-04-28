using Orion.Core.Irc.Server.Data.Internal;
using Orion.Core.Irc.Server.Data.Sessions;
using Orion.Core.Irc.Server.Events.Irc.Users;
using Orion.Core.Irc.Server.Handlers.Base;
using Orion.Core.Irc.Server.Interfaces.Listeners.Commands;
using Orion.Core.Irc.Server.Interfaces.Services.Irc;
using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Listeners.EventBus;
using Orion.Foundations.Types;
using Orion.Irc.Core.Commands;
using Orion.Irc.Core.Commands.Errors;
using Orion.Irc.Core.Commands.Replies;

namespace Orion.Server.Handlers;

public class UserHandler
    : BaseIrcCommandListener, IEventBusListener<UserNickNameChangeEvent>, IIrcCommandHandler<IsonCommand>
{
    private readonly IChannelManagerService _channelManagerService;

    public UserHandler(
        ILogger<UserHandler> logger, IrcCommandListenerContext context, IChannelManagerService channelManagerService
    ) : base(logger, context)
    {
        _channelManagerService = channelManagerService;
        SubscribeToEventBus(this);
        RegisterCommandHandler<IsonCommand>(this, ServerNetworkType.Clients);
    }

    public async Task HandleAsync(UserNickNameChangeEvent @event, CancellationToken cancellationToken = default)
    {
        var session = GetSession(@event.SessionId);


        var existingSession = GetSessionByNickName(@event.NewNickName);

        if (existingSession != null)
        {
            await session.SendCommandAsync(
                ErrNicknameInUse.Create(
                    ServerHostName,
                    session.NickName,
                    @event.NewNickName
                )
            );
            return;
        }

        session.NickName = @event.NewNickName;

        _channelManagerService.UpdateNickName(@event.OldNickName, @event.NewNickName);

        await session.SendCommandAsync(
            NickCommand.CreateChangeNotification(@event.OldNickName, @event.NewNickName)
        );

        var connectedUsers = await _channelManagerService.GetConnectedUsersAsync(@event.NewNickName);

        foreach (var user in connectedUsers)
        {
            var userSession = GetSessionByNickName(user);
            await userSession.SendCommandAsync(
                NickCommand.CreateChangeNotification(@event.OldNickName, @event.NewNickName)
            );
        }


        Logger.LogInformation(
            "User {OldNickName} changed nickname to {NewNickName}",
            @event.OldNickName,
            @event.NewNickName
        );
    }

    public async Task OnCommandReceivedAsync(
        IrcUserSession session, ServerNetworkType serverNetworkType, IsonCommand command
    )
    {
        var rplIson = RplIson.CreateEmpty(
            ServerHostName,
            session.NickName
        );

        foreach (var nickname in command.Nicknames)
        {
            if (GetSessionByNickName(nickname) != null)
            {
                rplIson.OnlineNicknames.Add(nickname);
            }
        }


        if (rplIson.OnlineNicknames.Count > 0)
        {
            await session.SendCommandAsync(rplIson);
            return;
        }

        await session.SendCommandAsync(
            ErrNoSuchNick.Create(
                ServerHostName,
                session.NickName,
                command.Nicknames.FirstOrDefault() ?? string.Empty
            )
        );
    }
}
