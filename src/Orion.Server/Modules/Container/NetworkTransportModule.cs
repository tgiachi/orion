using Orion.Core.Server.Extensions;
using Orion.Core.Server.Interfaces.Modules;

using Orion.Network.Core.Interfaces.Services;
using Orion.Network.Core.Services;

namespace Orion.Server.Modules.Container;

public class NetworkTransportModule : IOrionContainerModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        return services.AddService<INetworkTransportManager, NetworkTransportManager>();
    }
}
