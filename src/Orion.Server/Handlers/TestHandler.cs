using Orion.Core.Server.Interfaces.Listeners;
using Orion.Core.Server.Interfaces.Services;
using Orion.Core.Types;
using Orion.Irc.Core.Commands;
using Orion.Irc.Core.Interfaces.Commands;
using Orion.Server.Services;

namespace Orion.Server.Handlers;

public class TestHandler : IIrcCommandListener
{
    private readonly IIrcCommandService _ircCommandService;

    public TestHandler(IIrcCommandService ircCommandService)
    {
        _ircCommandService = ircCommandService;

        _ircCommandService.AddListener<NickCommand>(this, ServerNetworkType.Clients);
    }

    public Task OnCommandReceivedAsync(string sessionId, IIrcCommand command)
    {
        return Task.CompletedTask;
    }
}
