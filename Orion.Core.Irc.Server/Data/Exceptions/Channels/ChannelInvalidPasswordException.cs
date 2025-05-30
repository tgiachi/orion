namespace Orion.Core.Irc.Server.Data.Exceptions.Channels;

public class ChannelInvalidPasswordException : Exception
{
    public ChannelInvalidPasswordException(string channelName)
        : base($"Invalid password for channel: {channelName}")
    {
    }

    public ChannelInvalidPasswordException(string channelName, Exception innerException)
        : base($"Invalid password for channel: {channelName}", innerException)
    {
    }

}
