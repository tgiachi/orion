# Orion.Core.Server

![NuGet Version](https://img.shields.io/nuget/v/Orion.Core.Server)
![License](https://img.shields.io/badge/license-MIT-green)
![.NET](https://img.shields.io/badge/.NET-9.0-purple)

Server-side core functionality for the Orion IRC Server project.

> IRC is not dead, long live IRC!

## About

Orion.Core.Server provides the essential server-side infrastructure for building a full-featured IRC server. This library includes services, interfaces, configuration handling, event dispatching, and the modular architecture that makes Orion IRC Server flexible and extensible.

## Installation

```bash
dotnet add package Orion.Core.Server
```

Or using the Package Manager Console:

```
Install-Package Orion.Core.Server
```

## Key Features

- **Modular Architecture**: Build server components independently and register them during startup
- **Extensible Service Container**: Easy-to-use dependency injection system
- **Event System**: Built-in publish/subscribe event system using HyperCube.Postman
- **Configuration Management**: YAML-based configuration with automatic file handling
- **Diagnostic Services**: Performance metrics and system diagnostics
- **Process Queue**: Controlled processing of tasks with statistics
- **Scheduler**: Time-based task execution system
- **JavaScript Scripting Engine**: Extend the server with JavaScript
- **Text Templating**: Template processing with variable support

## Architecture

Orion.Core.Server is designed with a modular, service-oriented architecture:

- **Services**: Core functionality exposed through interfaces
- **Modules**: Components that register services with the container
- **Events**: Communication between loosely coupled components
- **Configuration**: Hierarchical, section-based server configuration
- **Command Listeners**: Process incoming IRC commands and events

## Examples

### Creating a Module

```csharp
using Orion.Core.Server.Extensions;
using Orion.Core.Server.Interfaces.Modules;

public class MyModule : IOrionContainerModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        return services
            .AddService<IMyService, MyService>()
            .AddIrcCommand<MyCommand>();
    }
}
```

### Creating a Service

```csharp
using Orion.Core.Server.Interfaces.Services.Base;

public interface IMyService : IOrionService
{
    Task DoSomethingAsync();
}

public class MyService : IMyService, IOrionStartService
{
    private readonly ILogger _logger;

    public MyService(ILogger<MyService> logger)
    {
        _logger = logger;
    }

    public Task DoSomethingAsync()
    {
        _logger.LogInformation("Doing something");
        return Task.CompletedTask;
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Service starting");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Service stopping");
        return Task.CompletedTask;
    }
}
```

### Working with Events

```csharp
using HyperCube.Postman.Interfaces.Services;
using Orion.Core.Server.Events.Server;

// Publishing an event
await _hyperPostmanService.PublishAsync(new ServerReadyEvent());

// Subscribing to events
public class MyEventHandler : ILetterListener<ServerReadyEvent>
{
    public async Task HandleAsync(ServerReadyEvent @event, CancellationToken cancellationToken = default)
    {
        // Handle the server ready event
    }
}
```

### Configuration

```csharp
using Orion.Core.Server.Data.Config.Base;

public class MyConfigSection : BaseConfigSection
{
    public string Name { get; set; } = "Default";
    public int SomeValue { get; set; } = 42;

    public override void Load()
    {
        // Custom loading logic
    }
}
```

## Dependencies

- **Orion.Core**: Core utilities and extensions
- **Orion.Irc.Core**: IRC protocol implementation
- **Orion.Network.Core**: Networking abstractions
- **HyperCube.Postman**: Event dispatching
- **Serilog**: Logging
- **CommandLineParser**: Command-line parsing

## Related Packages

- **Orion.Core.Server.Web**: Web API and HTTP interface
- **Orion.Network.Tcp**: TCP implementation for network transports

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Project Links

- GitHub: [https://github.com/tgiachi/orion](https://github.com/tgiachi/orion)
- NuGet: [https://www.nuget.org/packages/Orion.Core.Server](https://www.nuget.org/packages/Orion.Core.Server)
