namespace Orion.Core.Irc.Server.Events.Irc.Channels;

public record ChannelCreatedEvent(string ChannelName, string CreatedByNickName);
