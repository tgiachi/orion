using HyperCube.Postman.Base.Events;

namespace Orion.Core.Server.Events.TextTemplate;

public record AddVariableBuilderEvent(string VariableName, Func<object> Builder) : BasePostmanRecordEvent;
