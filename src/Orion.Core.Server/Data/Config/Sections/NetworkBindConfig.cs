using Orion.Core.Types;

namespace Orion.Core.Server.Data.Config.Sections;

public class NetworkBindConfig
{
    public string Host { get; set; } = "*";

    public ServerNetworkType NetworkType { get; set; } = ServerNetworkType.Clients;

    public string Ports { get; set; } = "6660-6669,6697";

    public bool UseSSL { get; set; } = false;
}
