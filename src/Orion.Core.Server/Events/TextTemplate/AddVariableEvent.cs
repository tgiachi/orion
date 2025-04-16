using HyperCube.Postman.Base.Events;

namespace Orion.Core.Server.Events.TextTemplate;

public record AddVariableEvent(string VariableName, object Value) : BasePostmanRecordEvent;
