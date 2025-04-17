using Orion.Core.Server.Handlers.Base;
using Orion.Core.Server.Interfaces.Listeners;
using Orion.Core.Server.Interfaces.Services.Irc;
using Orion.Core.Types;
using Orion.Irc.Core.Commands;
using Orion.Irc.Core.Commands.Replies;
using Orion.Irc.Core.Interfaces.Commands;


namespace Orion.Server.Handlers;

public class TestHandler : BaseIrcCommandListener, IIrcCommandListener<UserCommand>
{
    public TestHandler(ILogger<BaseIrcCommandListener> logger, IIrcCommandService ircCommandService) : base(
        logger,
        ircCommandService
    )
    {
        RegisterHandler<UserCommand>(this, ServerNetworkType.Clients);
    }

    public async Task OnCommandReceivedAsync(string sessionId, ServerNetworkType serverNetworkType, UserCommand command)
    {
        Logger.LogInformation("Received Command: {Command}", command);

        await SendCommandAsync(sessionId, RplAdminMe.Create("irc.test", "test", "test"));
    }
}
