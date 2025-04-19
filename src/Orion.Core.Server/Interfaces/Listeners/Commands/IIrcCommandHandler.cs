using Orion.Core.Server.Data.Sessions;
using Orion.Foundations.Types;
using Orion.Irc.Core.Interfaces.Commands;

namespace Orion.Core.Server.Interfaces.Listeners.Commands;

public interface IIrcCommandHandler<in TCommand>
    where TCommand : IIrcCommand
{
    /// <summary>
    ///   Called when a command is received.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="serverNetworkType"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    Task OnCommandReceivedAsync(IrcUserSession session, ServerNetworkType serverNetworkType, TCommand command);
}
