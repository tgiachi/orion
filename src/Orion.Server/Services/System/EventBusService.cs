using System.Collections.Concurrent;
using System.Reactive.Subjects;
using System.Threading.Tasks.Dataflow;
using Orion.Core.Server.Data.Config.Sections;
using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Interfaces.Services.System;
using Orion.Core.Server.Listeners.EventBus;

namespace Orion.Server.Services.System;

public class EventBusService : IEventBusService
{
    private readonly ILogger _logger;
    private readonly ConcurrentDictionary<Type, object> _listeners = new();
    private readonly ActionBlock<EventDispatchJob> _dispatchBlock;
    private readonly CancellationTokenSource _cts = new();

    private readonly Subject<object> _allEventsSubject = new Subject<object>();

    /// <summary>();
    /// Observable  that emits all events
    /// </summary>
    public IObservable<object> AllEventsObservable => _allEventsSubject;

    public EventBusService(ILogger<EventBusService> logger, EventBusConfig config)
    {
        _logger = logger;
        var executionOptions = new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = config.MaxConcurrentTasks,
            CancellationToken = _cts.Token
        };

        _dispatchBlock = new ActionBlock<EventDispatchJob>(
            job => job.ExecuteAsync(),
            executionOptions
        );

        _logger.LogInformation(
            "Signal emitter initialized with {ParallelTasks} dispatch tasks",
            config.MaxConcurrentTasks
        );
    }

    /// <summary>
    /// Register a listener for a specific event type
    /// </summary>
    public void Subscribe<TEvent>(IEventBusListener<TEvent> listener) where TEvent : class
    {
        var eventType = typeof(TEvent);

        // Get or create a list of listeners for this event type
        var listeners = (ConcurrentBag<IEventBusListener<TEvent>>)_listeners.GetOrAdd(
            eventType,
            _ => new ConcurrentBag<IEventBusListener<TEvent>>()
        );

        listeners.Add(listener);

        _logger.LogTrace(
            "Registered listener {ListenerType} for event {EventType}",
            listener.GetType().Name,
            eventType.Name
        );
    }

    /// <summary>
    /// Register a function as a listener for a specific event type
    /// </summary>
    public void Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class
    {
        var listener = new FunctionSignalListener<TEvent>(handler);
        Subscribe<TEvent>(listener);

        _logger.LogTrace(
            "Registered function handler for event {EventType}",
            typeof(TEvent).Name
        );
    }

    /// <summary>
    /// Unregisters a listener for a specific event type
    /// </summary>
    public void Unsubscribe<TEvent>(IEventBusListener<TEvent> listener)
        where TEvent : class
    {
        var eventType = typeof(TEvent);

        if (_listeners.TryGetValue(eventType, out var listenersObj))
        {
            var listeners = (ConcurrentBag<IEventBusListener<TEvent>>)listenersObj;

            // Create a new bag without the listener
            var updatedListeners = new ConcurrentBag<IEventBusListener<TEvent>>(
                listeners.Where(l => !ReferenceEquals(l, listener))
            );

            _listeners.TryUpdate(eventType, updatedListeners, listeners);

            _logger.LogTrace(
                "Unregistered listener {ListenerType} from event {EventType}",
                listener.GetType().Name,
                eventType.Name
            );
        }
    }

    /// <summary>
    /// Unregisters a function handler for a specific event type
    /// </summary>
    public void Unsubscribe<TEvent>(Func<TEvent, Task> handler)
        where TEvent : class
    {
        var eventType = typeof(TEvent);

        if (_listeners.TryGetValue(eventType, out var listenersObj))
        {
            var listeners = (ConcurrentBag<IEventBusListener<TEvent>>)listenersObj;

            // Create a new bag without the function handler
            var updatedListeners = new ConcurrentBag<IEventBusListener<TEvent>>(
                listeners.Where(l => !(l is FunctionSignalListener<TEvent> functionListener) ||
                                     !functionListener.HasSameHandler(handler)
                )
            );

            _listeners.TryUpdate(eventType, updatedListeners, listeners);

            _logger.LogTrace(
                "Unregistered function handler for event {EventType}",
                eventType.Name
            );
        }
    }

    /// <summary>
    /// Emits an event to all registered listeners asynchronously
    /// </summary>
    public async Task PublishAsync<TEvent>(TEvent eventData, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        var eventType = typeof(TEvent);

        _allEventsSubject.OnNext(eventData);

        if (!_listeners.TryGetValue(eventType, out var listenersObj))
        {
            _logger.LogTrace("No listeners registered for event {EventType}", eventType.Name);
            return;
        }

        var listeners = (ConcurrentBag<IEventBusListener<TEvent>>)listenersObj;

        _logger.LogTrace(
            "Emitting event {EventType} to {ListenerCount} listeners",
            eventType.Name,
            listeners.Count
        );

        // Dispatch jobs to process the event for each listener
        foreach (var listener in listeners)
        {
            try
            {
                var job = new EventDispatchJob<TEvent>(listener, eventData);
                await _dispatchBlock.SendAsync(job, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error dispatching event {EventType} to listener {ListenerType}",
                    eventType.Name,
                    listener.GetType().Name
                );

                throw;
            }
        }
    }

    public int GetListenerCount()
    {
        var totalCount = 0;



        return totalCount;
    }

    public int GetListenerCount<TEvent>() where TEvent : class
    {
        if (_listeners.TryGetValue(typeof(TEvent), out var listenersObj))
        {
            var listeners = (ConcurrentBag<IEventBusListener<TEvent>>)listenersObj;
            return listeners.Count;
        }

        return 0;
    }

    /// <summary>
    /// Waits for all queued events to be processed
    /// </summary>
    public async Task WaitForCompletionAsync()
    {
        _dispatchBlock.Complete();
        await _dispatchBlock.Completion;
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}
