using Microsoft.Extensions.DependencyInjection;
using Orion.Core.Server.Extensions;
using Orion.Irc.Core.Data.Internal;
using Orion.Irc.Core.Interfaces.Commands;

namespace Orion.Core.Irc.Server.Extensions;

public static class AddIrcCommandToParserExtension
{
    public static IServiceCollection AddIrcCommand<T>(this IServiceCollection services)
        where T : class, IIrcCommand, new()
    {
        var command = new T();
        return services.AddToRegisterTypedList(new IrcCommandDefinitionData(command));
    }
}
