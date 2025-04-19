namespace Orion.Core.Server.Data.Config.Sections;

public class EventBusConfig
{
    /// <summary>
    /// Gets or sets the maximum number of concurrent tasks used for event dispatching.
    /// </summary>
    /// <remarks>
    /// This limits the parallelism of event handling. Set to 0 or a negative number
    /// to use the default level of parallelism (usually equal to Environment.ProcessorCount).
    /// </remarks>
    public int MaxConcurrentTasks { get; set; } = 2;
}
