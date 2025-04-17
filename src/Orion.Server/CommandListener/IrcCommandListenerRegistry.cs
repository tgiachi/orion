using Orion.Core.Server.Interfaces.Listeners;

using Orion.Foundations.Types;
using Orion.Irc.Core.Interfaces.Commands;

namespace Orion.Server.CommandListener;

public class IrcCommandListenerRegistry
{
    private readonly Dictionary<ServerNetworkType, Dictionary<string, List<IIrcCommandListener>>> _registry =
        new();

    public IrcCommandListenerRegistry()
    {
        foreach (var type in Enum.GetValues<ServerNetworkType>())
        {
            _registry[type] = new Dictionary<string, List<IIrcCommandListener>>();
        }
    }

    public void Register<TCommand>(
        ServerNetworkType networkType,
        string commandCode,
        IIrcCommandListener listener
    ) where TCommand : IIrcCommand
    {
        if (!_registry[networkType].TryGetValue(commandCode, out var listeners))
        {
            listeners = new List<IIrcCommandListener>();
            _registry[networkType][commandCode] = listeners;
        }

        listeners.Add((IIrcCommandListener)listener);
    }

    public IReadOnlyList<IIrcCommandListener> GetListeners(
        ServerNetworkType networkType,
        string commandCode
    )
    {
        if (_registry.TryGetValue(networkType, out var commandMap) &&
            commandMap.TryGetValue(commandCode, out var listeners))
        {
            return listeners;
        }

        return [];
    }
}
