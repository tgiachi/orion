using Orion.Foundations.Types;

namespace Orion.Core.Irc.Server.Events.Irc.Sessions;

public record SessionConnectedEvent(string SessionId, string Endpoint, ServerNetworkType NetworkType);
