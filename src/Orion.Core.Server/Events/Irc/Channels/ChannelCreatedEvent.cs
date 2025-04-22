namespace Orion.Core.Server.Events.Irc.Channels;

public record ChannelCreatedEvent(string ChannelName, string CreatedByNickName);
