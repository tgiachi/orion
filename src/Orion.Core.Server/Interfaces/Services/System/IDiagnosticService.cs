using Orion.Core.Server.Data.Metrics.Diagnostic;
using Orion.Core.Server.Interfaces.Metrics;
using Orion.Core.Server.Interfaces.Services.Base;

namespace Orion.Core.Server.Interfaces.Services.System;

public interface IDiagnosticService : IOrionService, IOrionStartService
{
    /// <summary>
    ///  Get the current metrics
    /// </summary>
    /// <returns></returns>
    Task<List<MetricProviderData>> GetCurrentMetricsAsync();

    /// <summary>
    ///  Get the current metrics
    /// </summary>
    IObservable<MetricProviderData> Metrics { get; }


    /// <summary>
    ///  Get Pid file path
    /// </summary>
    string PidFilePath { get; }

    /// <summary>
    /// Collect metrics from all registered providers
    /// </summary>
    /// <returns></returns>
    Task CollectMetricsAsync();


    /// <summary>
    /// Register a provider of metrics
    /// </summary>
    /// <param name="provider">The metrics provider to register</param>
    void RegisterMetricsProvider(IMetricsProvider provider);

    /// <summary>
    /// Unregister a provider of metrics
    /// </summary>
    /// <param name="providerName">The name of the provider to unregister</param>
    void UnregisterMetricsProvider(string providerName);

    /// <summary>
    /// Get all metrics from registered providers
    /// </summary>
    /// <returns>Dictionary with provider names as keys and metrics objects as values</returns>
    Dictionary<string, object> GetAllProvidersMetrics();
}
