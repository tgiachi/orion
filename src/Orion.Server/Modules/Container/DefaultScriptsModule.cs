using Orion.Core.Server.Extensions;
using Orion.Core.Server.Interfaces.Modules;
using Orion.Server.Modules.Scripts;

namespace Orion.Server.Modules.Container;

public class DefaultScriptsModule : IOrionContainerModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        return services.AddScriptModule<JsLoggerModule>();
    }
}
