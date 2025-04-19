using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Data.Sessions;
using Orion.Core.Server.Handlers.Base;
using Orion.Core.Server.Interfaces.Listeners;
using Orion.Core.Server.Interfaces.Listeners.Commands;
using Orion.Foundations.Types;
using Orion.Irc.Core.Commands;


namespace Orion.Server.Handlers;

public class TestHandler : BaseIrcCommandListener, IIrcCommandHandler<UserCommand>
{
    public TestHandler(
        ILogger<BaseIrcCommandListener> logger, IrcCommandListenerContext context
    ) : base(
        logger,
        context
    )
    {
        RegisterCommandHandler<UserCommand>(this, ServerNetworkType.Clients);
    }


    public async Task OnCommandReceivedAsync(
        IrcUserSession session, ServerNetworkType serverNetworkType, UserCommand command
    )
    {
        Logger.LogInformation("Received Command: {Command}", command);

        // await session.SendCommandAsync(RplAdminMe.Create("irc.test", "test", "test"));
    }
}
