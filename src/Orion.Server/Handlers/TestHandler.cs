using Orion.Core.Server.Interfaces.Listeners;
using Orion.Core.Server.Interfaces.Services;
using Orion.Core.Server.Interfaces.Services.Irc;
using Orion.Core.Types;
using Orion.Irc.Core.Commands;
using Orion.Irc.Core.Commands.Replies;
using Orion.Irc.Core.Interfaces.Commands;
using Orion.Server.Services;

namespace Orion.Server.Handlers;

public class TestHandler : IIrcCommandListener
{
    private readonly IIrcCommandService _ircCommandService;

    private readonly ILogger _logger;

    public TestHandler(IIrcCommandService ircCommandService, ILogger<TestHandler> logger)
    {
        _ircCommandService = ircCommandService;
        _logger = logger;

        _ircCommandService.AddListener<NickCommand>(this, ServerNetworkType.Clients);
    }

    public async Task OnCommandReceivedAsync(string sessionId, IIrcCommand command)
    {

        _logger.LogInformation("Received Command: {Command}", command);
        await _ircCommandService.SendCommandAsync(sessionId, RplAdminMe.Create("irc.test", "test", "test"));
    }
}
