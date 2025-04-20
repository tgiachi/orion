using Microsoft.Extensions.DependencyInjection;
using Orion.Core.Server.Data.Config;
using Orion.Core.Server.Data.Internal;

namespace Orion.Core.Server.Extensions;

public static class CreateIrcServerContextExtension
{
    public static IServiceCollection CreateIrcServerAppContext(
        this IServiceCollection services, OrionServerConfig serverConfig
    )
    {
        services.AddSingleton(
            new IrcServerContextData
            {
                ServerName = serverConfig.Server.Host,
                NetworkName = serverConfig.Server.Network
            }
        );


        return services;
    }
}
