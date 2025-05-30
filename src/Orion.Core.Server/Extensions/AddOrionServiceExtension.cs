using Microsoft.Extensions.DependencyInjection;
using Orion.Core.Server.Interfaces.Services.Base;
using Orion.Core.Server.Internal;
using Orion.Core.Server.Internal.Services;
using Orion.Foundations.Types;

namespace Orion.Core.Server.Extensions;

/// <summary>
///     Extension methods for adding Orion services.
/// </summary>
public static class AddOrionServiceExtension
{
    public static IServiceCollection AddService(
        this IServiceCollection services, Type serviceType, Type implementationType,
        ServiceLifetimeType lifetimeType = ServiceLifetimeType.Singleton, int priority = 0
    )
    {
        var lifetime = lifetimeType switch
        {
            ServiceLifetimeType.Singleton => ServiceLifetime.Singleton,
            ServiceLifetimeType.Scoped    => ServiceLifetime.Scoped,
            ServiceLifetimeType.Transient => ServiceLifetime.Transient,
            _                             => throw new ArgumentOutOfRangeException(nameof(lifetimeType), lifetimeType, null)
        };

        var isAutoStart = typeof(IOrionStartService).IsAssignableFrom(implementationType);
        // check for already registered service

        if (services.Any(s => s.ServiceType == serviceType && s.ImplementationType == implementationType))
        {
            return services;
        }

        services.Add(new ServiceDescriptor(serviceType, implementationType, lifetime));

        services.AddToRegisterTypedList(
            new ServiceDefinitionObject(
                serviceType,
                implementationType,
                lifetimeType,
                isAutoStart,
                priority
            )
        );

        return services;
    }

    public static IServiceCollection AddService<TService, TImplementation>(
        this IServiceCollection services,
        ServiceLifetimeType lifetimeType = ServiceLifetimeType.Singleton, int priority = 0
    )
        where TService : class
        where TImplementation : class, TService
    {
        return services.AddService(typeof(TService), typeof(TImplementation), lifetimeType, priority);
    }

    public static IServiceCollection AddService<TService>(
        this IServiceCollection services,
        ServiceLifetimeType lifetimeType = ServiceLifetimeType.Singleton, int priority = 0
    )
        where TService : class

    {
        return services.AddService(typeof(TService), typeof(TService), lifetimeType, priority);
    }
}
