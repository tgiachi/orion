using Orion.Core.Server.Data.Config.Base;

namespace Orion.Core.Server.Data.Config.Sections;

public class OperConfig : BaseConfigSection
{
    public List<OperEntryConfig> Entries { get; set; } = new();


}
