using Orion.Core.Server.Data.Metrics.Diagnostic;
using Orion.Core.Server.Interfaces.Services.Base;

namespace Orion.Core.Server.Interfaces.Services;

public interface IDiagnosticService : IOrionStartService
{
    // Get current metrics
    Task<DiagnosticMetrics> GetCurrentMetricsAsync();

    // Observable for continuous monitoring
    IObservable<DiagnosticMetrics> Metrics { get; }

    // Get the PID file path
    string PidFilePath { get; }

    // Force collect diagnostics now
    Task CollectMetricsAsync();
}
