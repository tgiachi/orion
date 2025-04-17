using Orion.Core.Server.Data.Config.Base;


namespace Orion.Core.Server.Data.Config.Sections;

public class DebugConfig : BaseConfigSection
{
    public bool SaveRawPackets { get; set; } = false;
}
