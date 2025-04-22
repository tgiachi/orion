namespace Orion.Core.Server.Events.Irc.Users;

public record UserJoinChannelEvent(string NickName, string ChannelName);
