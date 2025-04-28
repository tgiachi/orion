namespace Orion.Core.Irc.Server.Events.Irc.Channels;

public record ChannelDeletedEvent(string ChannelName, string? NickName);
