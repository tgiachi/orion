using Microsoft.Extensions.DependencyInjection;
using Orion.Core.Server.Interfaces.Services.System;
using Orion.Core.Server.Interfaces.Sessions;
using Orion.Core.Server.Services;

namespace Orion.Core.Server.Extensions;

public static class AddNetworkSessionServiceExtension
{
    /// <summary>
    ///  Adds the network session service to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="TSession"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddNetworkSessionService<TSession>(this IServiceCollection services)
        where TSession : class, INetworkSession, new()
    {
        services.AddSingleton<INetworkSessionService<TSession>, NetworkSessionService<TSession>>();
        return services;
    }
}
