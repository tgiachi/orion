namespace Orion.Core.Irc.Server.Events.Irc.Users;

public record UserJoinChannelEvent(string NickName, string ChannelName);
