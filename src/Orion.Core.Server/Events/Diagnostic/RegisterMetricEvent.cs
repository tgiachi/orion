using Orion.Core.Server.Interfaces.Metrics;

namespace Orion.Core.Server.Events.Diagnostic;

public record RegisterMetricEvent(IMetricsProvider provider);
