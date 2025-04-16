using Orion.Irc.Core.Interfaces.Commands;

namespace Orion.Irc.Core.Interfaces.Parser;

public interface IIrcCommandParser
{
    Task<IIrcCommand> ParseAsync(string message);

    Task<string> SerializeAsync(IIrcCommand command);

    void RegisterCommand<TCommand>() where TCommand : IIrcCommand, new();

}
