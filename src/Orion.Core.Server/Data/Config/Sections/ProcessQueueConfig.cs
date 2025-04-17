using Orion.Core.Server.Data.Config.Base;

namespace Orion.Core.Server.Data.Config.Sections;

public class ProcessQueueConfig : BaseConfigSection
{
    public int MaxParallelTask { get; set; } = 2;
}
