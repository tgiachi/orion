using HyperCube.Postman.Base.Events;
using Orion.Core.Types;

namespace Orion.Core.Server.Events.Irc;

public record SessionConnectedEvent(string Id, string Endpoint, ServerNetworkType NetworkType) : BasePostmanRecordEvent;
