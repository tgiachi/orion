using HyperCube.Postman.Base.Events;
using Orion.Foundations.Types;

namespace Orion.Core.Server.Events.Irc;

public record SessionConnectedEvent(string SessionId, string Endpoint, ServerNetworkType NetworkType) : BasePostmanRecordEvent;
