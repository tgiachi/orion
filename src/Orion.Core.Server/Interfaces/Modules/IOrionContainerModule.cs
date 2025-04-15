using Microsoft.Extensions.DependencyInjection;

namespace Orion.Core.Server.Interfaces.Modules;

public interface IOrionContainerModule
{
    /// <summary>
    ///     Register the services in the container.
    /// </summary>
    /// <param name="container">The container to register the services in.</param>
    IServiceCollection RegisterServices(IServiceCollection serviceCollection);
}
