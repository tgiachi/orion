using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Data.Sessions;
using Orion.Core.Server.Handlers.Base;
using Orion.Core.Server.Interfaces.Listeners.Commands;
using Orion.Foundations.Types;
using Orion.Irc.Core.Commands;

namespace Orion.Server.Handlers;

public class WhoHandler : BaseIrcCommandListener, IIrcCommandHandler<WhoCommand>, IIrcCommandHandler<WhoIsCommand>
{
    public WhoHandler(ILogger<WhoHandler> logger, IrcCommandListenerContext context) : base(logger, context)
    {
        RegisterCommandHandler<WhoCommand>(this, ServerNetworkType.Clients);
        RegisterCommandHandler<WhoIsCommand>(this, ServerNetworkType.Clients);
    }

    public async Task OnCommandReceivedAsync(IrcUserSession session, ServerNetworkType serverNetworkType, WhoCommand command)
    {

    }

    public async Task OnCommandReceivedAsync(IrcUserSession session, ServerNetworkType serverNetworkType, WhoIsCommand command)
    {

    }
}
