namespace Orion.Core.Server.Events.Irc.Users;

public record UserQuitEvent(string NickName, string Message);
