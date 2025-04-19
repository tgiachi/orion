using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Events.Users;
using Orion.Core.Server.Handlers.Base;
using Orion.Core.Server.Interfaces.Listeners.EventBus;
using Orion.Irc.Core.Commands.Replies;

namespace Orion.Server.Handlers;

public class WelcomeHandler : BaseIrcCommandListener, IEventBusListener<UserAuthenticatedEvent>
{
    public WelcomeHandler(ILogger<WelcomeHandler> logger, IrcCommandListenerContext context) : base(logger, context)
    {
        SubscribeToEventBus(this);
    }

    public async Task HandleAsync(UserAuthenticatedEvent @event, CancellationToken cancellationToken = default)
    {
        var session = GetSession(@event.SessionId);


        await session.SendCommandAsync(
            RplWelcome.Create(
                ServerContextData.ServerName,
                session.NickName,
                ServerContextData.NetworkName,
                session.FullAddress
            )
        );

        await session.SendCommandAsync(
            (RplYourHost.Create(ServerContextData.ServerName, session.NickName, "orionirc-server v1.0.0.0"))
        );


    }
}
