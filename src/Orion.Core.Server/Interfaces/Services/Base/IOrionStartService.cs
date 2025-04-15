namespace Orion.Core.Server.Interfaces.Services.Base;

/// <summary>
/// Defines a contract for services that can be loaded, started, and stopped within the HyperCube framework.
/// </summary>
/// <remarks>
/// Services implementing this interface can be managed by the HyperCube service container.
/// The framework will call StartAsync when the application starts and StopAsync when the application
/// is shutting down, allowing for proper initialization and cleanup of resources.
///
/// This interface is similar to IHostedService in ASP.NET Core but is specific to the HyperCube
/// framework and may be used in different contexts, including non-ASP.NET applications.
///
/// Implementers should ensure that:
/// - StartAsync performs any necessary initialization
/// - StopAsync properly releases resources and handles graceful shutdown
/// - Both methods properly respect the provided cancellation token
/// </remarks>
public interface IOrionStartService
{
    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task StopAsync(CancellationToken cancellationToken = default);
}
