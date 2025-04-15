namespace Orion.Core.Server.Interfaces.Services;

public interface IEventDispatcherService
{
    void SubscribeToEvent(string eventName, Action<object?> eventHandler);
    void UnsubscribeFromEvent(string eventName, Action<object?> eventHandler);
}
