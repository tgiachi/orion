using HyperCube.Postman.Interfaces.Services;
using Orion.Core.Server.Data.Sessions;
using Orion.Core.Server.Handlers.Base;
using Orion.Core.Server.Interfaces.Listeners;
using Orion.Core.Server.Interfaces.Services.Irc;
using Orion.Core.Types;
using Orion.Irc.Core.Commands;
using Orion.Irc.Core.Commands.Replies;
using Orion.Irc.Core.Interfaces.Commands;


namespace Orion.Server.Handlers;

public class TestHandler : BaseIrcCommandListener, IIrcCommandHandler<UserCommand>
{
    public TestHandler(
        ILogger<BaseIrcCommandListener> logger, IIrcCommandService ircCommandService, IHyperPostmanService postmanService,
        IIrcSessionService sessionService
    ) : base(
        logger,
        ircCommandService,
        postmanService,
        sessionService
    )
    {
        RegisterCommandHandler<UserCommand>(this, ServerNetworkType.Clients);
    }


    public async Task OnCommandReceivedAsync(
        IrcUserSession session, ServerNetworkType serverNetworkType, UserCommand command
    )
    {
        Logger.LogInformation("Received Command: {Command}", command);

        await session.SendCommandAsync(RplAdminMe.Create("irc.test", "test", "test"));
    }
}
