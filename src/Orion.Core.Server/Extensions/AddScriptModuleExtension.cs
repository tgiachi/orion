using Microsoft.Extensions.DependencyInjection;
using Orion.Core.Server.Data.Internal;

namespace Orion.Core.Server.Extensions;

public static class AddScriptModuleExtension
{
    public static IServiceCollection AddScriptModule<T>(this IServiceCollection services)
        where T : class
    {
        return services.AddScriptModule(typeof(T));
    }

    public static IServiceCollection AddScriptModule(this IServiceCollection services, Type type)
    {
        services.AddSingleton(type);
        return services.AddToRegisterTypedList(new ScriptModuleData(type));
    }
}
