# Event System

![Version](https://img.shields.io/badge/version-0.4.0-blue)
![License](https://img.shields.io/badge/license-MIT-green)
![.NET](https://img.shields.io/badge/.NET-9.0-purple)

## Overview

Orion IRC Server uses a powerful, flexible event system for communication between components. This publish-subscribe (pub-sub) architecture enables loose coupling between services and modules, making the system more maintainable and extensible.

The event system, implemented through the `IEventBusService` interface, serves as the central nervous system of Orion, allowing components to communicate without direct dependencies on each other.

## Core Components

### IEventBusService

The central interface for the event system:

```csharp
public interface IEventBusService
{
    IObservable<object> AllEventsObservable { get; }

    void Subscribe<TEvent>(IEventBusListener<TEvent> listener) where TEvent : class;
    void Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class;
    void Unsubscribe<TEvent>(IEventBusListener<TEvent> listener) where TEvent : class;

    Task PublishAsync<TEvent>(TEvent eventData, CancellationToken cancellationToken = default) where TEvent : class;

    int GetListenerCount();
    int GetListenerCount<TEvent>() where TEvent : class;

    Task WaitForCompletionAsync();
}
```

### IEventBusListener

Interface for event listeners:

```csharp
public interface IEventBusListener<TEvent>
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}
```

### EventBusService

The primary implementation of `IEventBusService`:

```csharp
public class EventBusService : IEventBusService
{
    private readonly ConcurrentDictionary<Type, object> _listeners = new();
    private readonly ActionBlock<EventDispatchJob> _dispatchBlock;
    private readonly Subject<object> _allEventsSubject = new();

    // Implementation of the interface methods
}
```

### FunctionSignalListener

An adapter that allows using functions as event listeners:

```csharp
public class FunctionSignalListener<TEvent> : IEventBusListener<TEvent> where TEvent : class
{
    private readonly Func<TEvent, Task> _handler;

    public FunctionSignalListener(Func<TEvent, Task> handler)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public Task HandleAsync(TEvent signalEvent, CancellationToken cancellationToken = default)
    {
        return _handler(signalEvent);
    }

    public bool HasSameHandler(Func<TEvent, Task> handler)
    {
        return _handler.Equals(handler);
    }
}
```

### EventDispatchJob

Base class for dispatching events to listeners:

```csharp
public abstract class EventDispatchJob
{
    public abstract Task ExecuteAsync();
}

public class EventDispatchJob<TEvent> : EventDispatchJob where TEvent : class
{
    private readonly IEventBusListener<TEvent> _listener;
    private readonly TEvent _event;

    public EventDispatchJob(IEventBusListener<TEvent> listener, TEvent @event)
    {
        _listener = listener;
        _event = @event;
    }

    public override async Task ExecuteAsync()
    {
        await _listener.HandleAsync(_event);
    }
}
```

## Event Types

Orion defines various event types for different purposes:

### Server Events

Events related to server lifecycle:

```csharp
public record ServerStartedEvent;
public record ServerReadyEvent;
public record ServerStoppingEvent;
```

### Diagnostic Events

Events related to server diagnostics:

```csharp
public record DiagnosticMetricEvent(DiagnosticMetrics Metrics);
```

### Session Events

Events related to client sessions:

```csharp
public record SessionConnectedEvent(string SessionId, string Endpoint, ServerNetworkType NetworkType);
public record SessionDisconnectedEvent(string SessionId);
public record UserAuthenticatedEvent(string SessionId);
```

### Text Template Events

Events for managing text template variables:

```csharp
public record AddVariableEvent(string VariableName, object Value);
public record AddVariableBuilderEvent(string VariableName, Func<object> Builder);
```

### Scheduler Events

Events for managing scheduled jobs:

```csharp
public abstract record AddSchedulerJobEvent(string Name, TimeSpan TotalSpan, Func<Task> Action);
```

## Event Flow

1. **Event Definition**:
  - Create a record or class to represent the event
  - Include any necessary data in the event object

2. **Publishing Events**:
  - Components create event instances and publish them
  - Example: `await _eventBusService.PublishAsync(new ServerStartedEvent());`
  - The event is broadcast to all subscribed listeners

3. **Subscribing to Events**:
  - Components can subscribe to events in two ways:
    - Implementing `IEventBusListener<TEvent>` interface
    - Providing a function handler via `Subscribe<TEvent>(Func<TEvent, Task> handler)`
  - Example: `_eventBusService.Subscribe<ServerReadyEvent>(this);`

4. **Handling Events**:
  - Listeners receive event notifications via their `HandleAsync` method
  - Function handlers are invoked directly with the event data

5. **Event Dispatch**:
  - Events are dispatched to listeners through an `ActionBlock` for parallelism control
  - The `EventBusConfig.MaxConcurrentTasks` setting controls the level of parallelism

## Implementation Details

### Thread Safety

The event system uses `ConcurrentDictionary` and other thread-safe collections to ensure proper operation in a multi-threaded environment.

### Performance Considerations

- Events are dispatched asynchronously to avoid blocking publishers
- Configurable parallelism to balance resource usage and throughput
- Listeners can be added and removed dynamically at runtime

### Error Handling

- Exceptions in event handlers are caught and logged
- Failed event dispatches do not affect other listeners
- Event dispatch jobs continue processing even when some listeners fail

## Examples

### Creating an Event Listener

```csharp
public class NetworkService : INetworkService, IEventBusListener<ServerReadyEvent>
{
    private readonly IEventBusService _eventBusService;

    public NetworkService(IEventBusService eventBusService)
    {
        _eventBusService = eventBusService;
        _eventBusService.Subscribe(this);
    }

    public async Task HandleAsync(ServerReadyEvent @event, CancellationToken cancellationToken = default)
    {
        // Start network services when server is ready
        await StartNetworkAsync();
    }
}
```

### Publishing Events

```csharp
public class OrionHostedService : IHostedService
{
    private readonly IEventBusService _eventBusService;

    public OrionHostedService(IEventBusService eventBusService)
    {
        _eventBusService = eventBusService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Start all services

        // Notify that the server has started
        await _eventBusService.PublishAsync(new ServerStartedEvent(), cancellationToken);

        // Notify that the server is ready for connections
        await _eventBusService.PublishAsync(new ServerReadyEvent(), cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        // Notify that the server is stopping
        await _eventBusService.PublishAsync(new ServerStoppingEvent(), cancellationToken);

        // Stop all services
    }
}
```

### Using Function Handlers

```csharp
public class SomeService
{
    public SomeService(IEventBusService eventBusService)
    {
        // Subscribe with a function handler
        eventBusService.Subscribe<DiagnosticMetricEvent>(HandleDiagnostics);
    }

    private async Task HandleDiagnostics(DiagnosticMetricEvent @event)
    {
        var metrics = @event.Metrics;
        // Process the diagnostic metrics
        await Task.CompletedTask;
    }
}
```

## Configuration

The event system can be configured through the `EventBusConfig` class:

```csharp
public class EventBusConfig
{
    /// <summary>
    /// Gets or sets the maximum number of concurrent tasks used for event dispatching.
    /// </summary>
    /// <remarks>
    /// This limits the parallelism of event handling. Set to 0 or a negative number
    /// to use the default level of parallelism (usually equal to Environment.ProcessorCount).
    /// </remarks>
    public int MaxConcurrentTasks { get; set; } = 2;
}
```

## Benefits

- **Loose Coupling**: Components communicate without direct references to each other
- **Modularity**: Easy to add new event types and listeners
- **Extensibility**: New modules can hook into existing events without modifying code
- **Testability**: Event-based systems are easier to mock and test
- **Scalability**: Built-in control over concurrent event processing

## Conclusion

Orion's event system provides a robust foundation for communication between components. It enables a clean, modular architecture where concerns are properly separated, making the system more maintainable and easier to extend.
