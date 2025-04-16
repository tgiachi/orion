using Orion.Irc.Core.Interfaces.Commands;
using Orion.Network.Core.Interfaces.Services;

namespace Orion.Core.Server.Interfaces.Listeners;

public interface IIrcCommandListener<in TCommand>
    where TCommand : IIrcCommand
{
    /// <summary>
    ///     Called when a command is received.
    /// </summary>
    /// <param name="command">The command.</param>
    Task OnCommandReceivedAsync(string sessionId, TCommand command);
}
