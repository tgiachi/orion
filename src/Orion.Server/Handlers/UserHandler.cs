using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Events.Irc.Users;
using Orion.Core.Server.Handlers.Base;
using Orion.Core.Server.Interfaces.Listeners.EventBus;
using Orion.Core.Server.Interfaces.Services.Irc;
using Orion.Irc.Core.Commands;
using Orion.Irc.Core.Commands.Errors;

namespace Orion.Server.Handlers;

public class UserHandler : BaseIrcCommandListener, IEventBusListener<UserNickNameChangeEvent>
{
    private readonly IChannelManagerService _channelManagerService;

    public UserHandler(
        ILogger<UserHandler> logger, IrcCommandListenerContext context, IChannelManagerService channelManagerService
    ) : base(logger, context)
    {
        _channelManagerService = channelManagerService;
        SubscribeToEventBus(this);
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
}
