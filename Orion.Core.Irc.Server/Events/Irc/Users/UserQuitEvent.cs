namespace Orion.Core.Irc.Server.Events.Irc.Users;

public record UserQuitEvent(string NickName, string Message);
