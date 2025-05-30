namespace Orion.Core.Irc.Server.Data.Exceptions.Channels;

public class ChannelFullException : Exception
{
    public ChannelFullException(string channelName)
        : base($"Channel {channelName} is full.")
    {
    }

    public ChannelFullException(string channelName, Exception innerException)
        : base($"Channel {channelName} is full.", innerException)
    {
    }
}
