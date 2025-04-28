namespace Orion.Core.Irc.Server.Events.Irc.Channels;

public record JoinRequestEvent(string Nickname, params string[] Channels);
