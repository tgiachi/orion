using Orion.Core.Server.Data.Config.Base;
using Orion.Core.Server.Interfaces.Config;

namespace Orion.Core.Server.Data.Config.Sections;

public class ProcessQueueConfig : BaseConfigSection
{
    public int MaxParallelTask { get; set; } = 2;
}
