using Orion.Core.Server.Data.Config.Sections;

namespace Orion.Core.Server.Interfaces.Config;

public interface IOrionServerConfig
{
    DebugConfig Debug { get; set; }

    ProcessConfig Process { get; set; }
}
