using Microsoft.Extensions.DependencyInjection;
using Orion.Core.Irc.Server.Data.Internal;
using Orion.Core.Server.Extensions;

namespace Orion.Core.Irc.Server.Extensions;

public static class AddIrcListenerExtension
{

    public static IServiceCollection AddIrcListener(this IServiceCollection collection, Type type)
    {

        collection.AddSingleton(type);
        collection.AddToRegisterTypedList(new IrcListenerDefinition(type));

        return collection;
    }

    public static IServiceCollection AddIrcListener<T>(this IServiceCollection collection)
    {
        return AddIrcListener(collection, typeof(T));
    }

}
