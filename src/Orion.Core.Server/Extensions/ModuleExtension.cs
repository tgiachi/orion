using Microsoft.Extensions.DependencyInjection;
using Orion.Core.Server.Interfaces.Modules;

namespace Orion.Core.Server.Extensions;

/// <summary>
/// Extension methods for registering OrionIRC modules in the dependency injection container
/// </summary>
public static class ModuleExtension
{
    /// <summary>
    /// Adds a module of type T to the service collection
    /// </summary>
    /// <typeparam name="T">The module type that implements IOrionContainerModule and has a parameterless constructor</typeparam>
    /// <param name="services">The service collection to add the module to</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddModule<T>(this IServiceCollection services)
        where T : class, IOrionContainerModule, new()
    {
        var module = new T();
        return module.RegisterServices(services);
    }

    /// <summary>
    /// Adds a module of type T to the service collection using a factory method
    /// </summary>
    /// <typeparam name="T">The module type that implements IOrionContainerModule</typeparam>
    /// <param name="services">The service collection to add the module to</param>
    /// <param name="factory">A factory function that creates the module instance</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddModule<T>(this IServiceCollection services, Func<T> factory)
        where T : class, IOrionContainerModule
    {
        var module = factory();
        return module.RegisterServices(services);
    }

    /// <summary>
    /// Adds a module to the service collection using a factory method that returns IOrionContainerModule
    /// </summary>
    /// <param name="services">The service collection to add the module to</param>
    /// <param name="moduleFactory">A factory function that creates the module instance</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddModuleFactory(
        this IServiceCollection services,
        Func<IOrionContainerModule> moduleFactory
    )
    {
        var module = moduleFactory();
        return module.RegisterServices(services);
    }
}
