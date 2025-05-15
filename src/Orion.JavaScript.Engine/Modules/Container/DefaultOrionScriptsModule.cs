using Microsoft.Extensions.DependencyInjection;
using Orion.Core.Server.Interfaces.Modules;
using Orion.JavaScript.Engine.Extensions;
using Orion.JavaScript.Engine.Modules.Scripts;

namespace Orion.JavaScript.Engine.Modules.Container;

public class DefaultOrionScriptsModule : IOrionContainerModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        return services.AddScriptModule<JsLoggerModule>();
    }
}
