using Orion.Irc.Core.Data.Channels;

namespace Orion.Core.Server.Data.Channels;

public class ChannelJoinResult
{
    public ChannelData? ChannelData { get; set; }

    public bool IsSuccess { get; set; }

    public Exception? Exception { get; set; }
}
