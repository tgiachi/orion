using Orion.Core.Server.Data.Config.Sections;

namespace Orion.Core.Server.Data.Config;

public class OrionServerConfig
{
    public WebHttpConfig WebHttp { get; set; } = new();

    public JwtAuthConfig JwtAuth { get; set; } = new();

    public IrcServerConfig Irc { get; set; } = new();

    public NetworkConfig Network { get; set; } = new();
}
