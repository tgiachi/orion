using Orion.Irc.Core.Interfaces.Commands;

namespace Orion.Irc.Core.Commands.Base;

public abstract class BaseIrcCommand : IIrcCommand
{
    private readonly string _code;

    public string Code => _code;

    public virtual void Parse(string line)
    {
    }

    public virtual string Write()
    {
        return string.Empty;
    }

    protected BaseIrcCommand(string code)
    {
        _code = code;
    }
}
