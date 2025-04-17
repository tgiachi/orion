using HyperCube.Postman.Interfaces.Services;
using Orion.Core.Extensions;
using Orion.Core.Server.Interfaces.Services;
using Orion.Core.Server.Interfaces.Services.System;

namespace Orion.Server.Services.System;

public class EventDispatcherService : IEventDispatcherService
{
    private readonly Dictionary<string, List<Action<object?>>> _eventHandlers = new();

    private readonly ILogger _logger;

    public EventDispatcherService(ILogger<EventDispatcherService> logger, IHyperPostmanService postmanService)
    {
        _logger = logger;
        postmanService.AllEventsObservable.Subscribe(OnEvent);
    }

    private void OnEvent(object obj)
    {
        DispatchEvent(obj.GetType().Name.ToSnakeCase().Replace("_event", ""), obj);
    }


    private void DispatchEvent(string eventName, object? eventData = null)
    {
        _logger.LogDebug("Dispatching event {EventName}", eventName);
        if (!_eventHandlers.TryGetValue(eventName, out var eventHandler))
        {
            return;
        }

        foreach (var handler in eventHandler)
        {
            handler(eventData);
        }
    }

    public void SubscribeToEvent(string eventName, Action<object?> eventHandler)
    {
        if (!_eventHandlers.TryGetValue(eventName, out var eventHandlers))
        {
            eventHandlers = new List<Action<object?>>();
            _eventHandlers.Add(eventName, eventHandlers);
        }

        eventHandlers.Add(eventHandler);
    }

    public void UnsubscribeFromEvent(string eventName, Action<object?> eventHandler)
    {
        if (!_eventHandlers.TryGetValue(eventName, out var eventHandlers))
        {
            return;
        }

        eventHandlers.Remove(eventHandler);
    }
}
