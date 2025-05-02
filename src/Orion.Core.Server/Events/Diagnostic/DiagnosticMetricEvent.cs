using Orion.Core.Server.Data.Metrics.Diagnostic;


namespace Orion.Core.Server.Events.Diagnostic;

public record DiagnosticMetricEvent(MetricProviderData Metrics);
