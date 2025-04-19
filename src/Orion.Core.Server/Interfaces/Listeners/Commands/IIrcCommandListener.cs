using Orion.Foundations.Types;
using Orion.Irc.Core.Interfaces.Commands;

namespace Orion.Core.Server.Interfaces.Listeners.Commands;

public interface IIrcCommandListener
{
    /// <summary>
    ///     Called when a command is received.
    /// </summary>
    /// <param name="command">The command.</param>
    Task OnCommandReceivedAsync(string sessionId, ServerNetworkType serverNetworkType, IIrcCommand command);
}

public interface IIrcCommandListener<in TCommand> : IIrcCommandListener
    where TCommand : IIrcCommand
{
    /// <summary>
    ///   Called when a command is received.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="serverNetworkType"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    Task OnCommandReceivedAsync(string sessionId, ServerNetworkType serverNetworkType, TCommand command);
}


