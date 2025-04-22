using Orion.Foundations.Types;

namespace Orion.Core.Server.Events.Irc.Sessions;

public record SessionConnectedEvent(string SessionId, string Endpoint, ServerNetworkType NetworkType);
