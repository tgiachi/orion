using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Data.Sessions;
using Orion.Core.Server.Handlers.Base;
using Orion.Core.Server.Interfaces.Listeners.Commands;
using Orion.Foundations.Types;
using Orion.Irc.Core.Commands;
using Orion.Irc.Core.Commands.Replies;

namespace Orion.Server.Handlers;

public class ServerHandler
    : BaseIrcCommandListener, IIrcCommandHandler<TimeCommand>, IIrcCommandHandler<RestartCommand>,
        IIrcCommandHandler<RehashCommand>
{
    public ServerHandler(ILogger<ServerHandler> logger, IrcCommandListenerContext context) : base(logger, context)
    {
        RegisterCommandHandler<TimeCommand>(this, ServerNetworkType.Clients);
        RegisterCommandHandler<RestartCommand>(this, ServerNetworkType.Clients);
        RegisterCommandHandler<RehashCommand>(this, ServerNetworkType.Clients);
    }

    public async Task OnCommandReceivedAsync(
        IrcUserSession session, ServerNetworkType serverNetworkType, TimeCommand command
    )
    {
        await session.SendCommandAsync(
            RplTime.Create(
                ServerHostName,
                session.NickName,
                command.Target,
                DateTime.Now
            )
        );
    }

    public async Task OnCommandReceivedAsync(
        IrcUserSession session, ServerNetworkType serverNetworkType, RestartCommand command
    )
    {
    }

    public async Task OnCommandReceivedAsync(
        IrcUserSession session, ServerNetworkType serverNetworkType, RehashCommand command
    )
    {
        if (!session.IsOperator)
        {

        }
    }
}
