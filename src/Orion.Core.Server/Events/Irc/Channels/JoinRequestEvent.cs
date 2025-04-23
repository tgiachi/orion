namespace Orion.Core.Server.Events.Irc.Channels;

public record JoinRequestEvent(string Nickname, params string[] Channels);
