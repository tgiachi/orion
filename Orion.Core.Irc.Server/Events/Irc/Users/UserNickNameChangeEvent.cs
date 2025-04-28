namespace Orion.Core.Irc.Server.Events.Irc.Users;

public record UserNickNameChangeEvent(string SessionId, string OldNickName, string NewNickName);
