using Orion.Core.Server.Data.Config.Sections;

namespace Orion.Core.Server.Data.Config;

public class OrionServerConfig
{
    public string PidFile { get; set; } = "orion_server.pid";

    public WebHttpConfig WebHttp { get; set; } = new();

    public JwtAuthConfig JwtAuth { get; set; } = new();

    public ServerConfig Irc { get; set; } = new();

    public NetworkConfig Network { get; set; } = new();

    public OperConfig Opers { get; set; } = new();
}
