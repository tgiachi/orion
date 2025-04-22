namespace Orion.Core.Server.Events.Irc.Users;

public record UserNickNameChangeEvent(string SessionId, string OldNickName, string NewNickName);
