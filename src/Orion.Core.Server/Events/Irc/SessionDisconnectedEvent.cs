using HyperCube.Postman.Base.Events;

namespace Orion.Core.Server.Events.Irc;

public record SessionDisconnectedEvent(string SessionId) : BasePostmanRecordEvent;
