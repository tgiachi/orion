using Microsoft.Extensions.DependencyInjection;
using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Interfaces.Listeners;
using Orion.Irc.Core.Interfaces.Commands;

namespace Orion.Core.Server.Extensions;

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
