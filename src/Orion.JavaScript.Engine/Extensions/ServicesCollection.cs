using Microsoft.Extensions.DependencyInjection;
using Orion.Core.Server.Data.Config.Internal;
using Orion.Core.Server.Extensions;
using Orion.Core.Server.Interfaces.Services.System;
using Orion.JavaScript.Engine.Data.Configs;
using Orion.JavaScript.Engine.Services;

namespace Orion.JavaScript.Engine.Extensions;

public static class ServicesCollection
{
    public static IServiceCollection AddJsScriptEngineService(
        this IServiceCollection services, ScriptEngineConfig? config = null
    )
    {
        config ??= new ScriptEngineConfig();
        services.AddSingleton(config);
        return services.AddService<IScriptEngineService, ScriptEngineService>();
    }
}
