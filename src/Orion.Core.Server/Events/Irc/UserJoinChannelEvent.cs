namespace Orion.Core.Server.Events.Irc;

public record UserJoinChannelEvent(string NickName, string ChannelName);
