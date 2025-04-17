using Orion.Core.Server.Data.Config.Sections;
using Orion.Core.Server.Interfaces.Config;

namespace Orion.Core.Server.Data.Config;

public class OrionServerConfig : IOrionServerConfig
{
    public WebHttpConfig WebHttp { get; set; } = new();
    public ServerConfig Server { get; set; } = new();
    public NetworkConfig Network { get; set; } = new();
    public OperConfig Oper { get; set; } = new();
    public ProcessConfig Process { get; set; } = new();

    public DebugConfig Debug { get; set; } = new();

}
