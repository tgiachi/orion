using HyperCube.Postman.Base.Events;
using HyperCube.Postman.Interfaces.Events;
using Orion.Core.Server.Data.Metrics.Diagnostic;

namespace Orion.Core.Server.Events.Diagnostic;

public record DiagnosticMetricEvent(DiagnosticMetrics Metrics) : BasePostmanRecordEvent;
