namespace Orion.Irc.Core.Interfaces.Commands;

public interface IIrcCommand
{
    string Code { get; }

    void Parse(string line);

    string Write();
}
