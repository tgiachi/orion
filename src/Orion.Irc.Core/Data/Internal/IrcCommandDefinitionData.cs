using Orion.Irc.Core.Interfaces.Commands;

namespace Orion.Irc.Core.Data.Internal;

public class IrcCommandDefinitionData
{
    public IIrcCommand Command { get; set; }

    public IrcCommandDefinitionData(IIrcCommand command)
    {
        Command = command;
    }

    public IrcCommandDefinitionData()
    {

    }
}
