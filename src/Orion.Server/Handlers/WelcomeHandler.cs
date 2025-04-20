using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Events.Users;
using Orion.Core.Server.Handlers.Base;
using Orion.Core.Server.Interfaces.Listeners.EventBus;
using Orion.Core.Server.Interfaces.Services.System;
using Orion.Irc.Core.Commands.Replies;

namespace Orion.Server.Handlers;

public class WelcomeHandler : BaseIrcCommandListener, IEventBusListener<UserAuthenticatedEvent>
{
    private readonly IVersionService _versionService;

    public WelcomeHandler(
        ILogger<WelcomeHandler> logger, IrcCommandListenerContext context, IVersionService versionService
    ) : base(logger, context)
    {
        _versionService = versionService;
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
            (RplYourHost.Create(
                ServerContextData.ServerName,
                session.NickName,
                "orionirc-server " + _versionService.GetVersionInfo().Version
            ))
        );

        await session.SendCommandAsync(
            RplCreated.Create(ServerContextData.ServerName, session.NickName, null, ServerContextData.ServerStartTime)
        );
    }
}
