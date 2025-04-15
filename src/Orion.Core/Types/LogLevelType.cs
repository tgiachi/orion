namespace Orion.Core.Types;

/// <summary>
/// Defines the standard log levels used throughout the HyperCube framework.
/// </summary>
/// <remarks>
/// These log levels follow the standard logging conventions and provide a
/// framework-agnostic way to specify log severity, which can then be mapped
/// to specific logging providers.
/// </remarks>
public enum LogLevelType
{
    /// <summary>
    /// The most detailed level of logging. Used for tracing the execution flow.
    /// </summary>
    Trace,

    /// <summary>
    /// Used for information that is useful for debugging.
    /// More detailed than Information but less detailed than Trace.
    /// </summary>
    Debug,

    /// <summary>
    /// Standard information messages that highlight the progress of the application.
    /// </summary>
    Information,

    /// <summary>
    /// Potentially harmful situations that don't cause the application to fail.
    /// </summary>
    Warning,

    /// <summary>
    /// Error events that might still allow the application to continue running.
    /// </summary>
    Error
}
