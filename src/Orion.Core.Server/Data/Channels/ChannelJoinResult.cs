using Orion.Irc.Core.Data.Channels;
using Orion.Irc.Core.Interfaces.Commands;

namespace Orion.Core.Server.Data.Channels;

public class ChannelJoinResult
{
    public ChannelData? ChannelData { get; set; }

    public bool IsSuccess { get; set; }

    public Exception? Exception { get; set; }

    public List<IIrcCommand> JoinedUserCommands { get; set; } = new();

    public Dictionary<string, List<IIrcCommand>> MembersCommands { get; set; } = new();


    public void AddJoinedUserCommand(IIrcCommand command)
    {
        JoinedUserCommands.Add(command);
    }

    public void AddMemberCommand(string nickName, IIrcCommand command)
    {
        if (!MembersCommands.TryGetValue(nickName, out List<IIrcCommand>? value))
        {
            value = [];
            MembersCommands[nickName] = value;
        }

        value.Add(command);
    }
}
