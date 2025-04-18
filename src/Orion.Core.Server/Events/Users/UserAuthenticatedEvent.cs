using HyperCube.Postman.Base.Events;

namespace Orion.Core.Server.Events.Users;

public record UserAuthenticatedEvent(string SessionId) : BasePostmanRecordEvent;
