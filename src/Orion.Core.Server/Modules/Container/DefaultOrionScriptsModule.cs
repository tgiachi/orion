using Microsoft.Extensions.DependencyInjection;
using Orion.Core.Server.Extensions;
using Orion.Core.Server.Interfaces.Modules;
using Orion.Core.Server.Modules.Scripts;

namespace Orion.Core.Server.Modules.Container;

public class DefaultOrionScriptsModule : IOrionContainerModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        return services.AddScriptModule<JsLoggerModule>();
    }
}
