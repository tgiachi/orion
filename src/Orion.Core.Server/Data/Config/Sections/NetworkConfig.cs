using Orion.Core.Server.Data.Config.Base;
using Orion.Core.Server.Types;
using Orion.Core.Types;

namespace Orion.Core.Server.Data.Config.Sections;

public class NetworkConfig : BaseConfigSection
{
    public SSLConfig SSL { get; set; } = new();

    public List<NetworkBindConfig> Binds { get; set; } = new();


    public NetworkConfig()
    {
        Binds.Add(
            new NetworkBindConfig()
            {
                Host = "0.0.0.0",
                Ports = "6660-6661",
                NetworkType = ServerNetworkType.Clients,
                UseSSL = false
            }
        );
    }
}
