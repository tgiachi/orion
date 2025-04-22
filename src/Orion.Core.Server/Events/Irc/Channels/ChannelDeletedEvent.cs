namespace Orion.Core.Server.Events.Irc.Channels;

public record ChannelDeletedEvent(string ChannelName, string? NickName);
