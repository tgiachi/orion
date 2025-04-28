using Microsoft.Extensions.DependencyInjection;
using Orion.Core.Irc.Server.Data.Internal;
using Orion.Core.Server.Data.Config;

namespace Orion.Core.Irc.Server.Extensions;

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
