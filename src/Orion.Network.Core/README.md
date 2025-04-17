# Orion.Network.Core

![NuGet Version](https://img.shields.io/nuget/v/Orion.Network.Core)
![License](https://img.shields.io/badge/license-MIT-green)
![.NET](https://img.shields.io/badge/.NET-9.0-purple)

Networking abstractions for the Orion IRC Server project.

> IRC is not dead, long live IRC!

## About

Orion.Network.Core provides the foundational networking abstraction layer for Orion IRC Server. This library offers a transport-agnostic approach to network communication, allowing the IRC server to work with different transport mechanisms (TCP, WebSockets, etc.) through a unified interface.

## Installation

```bash
dotnet add package Orion.Network.Core
```

Or using the Package Manager Console:

```
Install-Package Orion.Network.Core
```

## Key Features

- **Transport Abstraction**: Interface-based design for network transports
- **Message Handling**: Unified message reception and dispatch
- **Connection Management**: Client connection tracking and lifecycle
- **Reactive Design**: Event-based communication using System.Reactive
- **Protocol-Independent**: Works with various network protocols
- **Network Data Types**: Strong typing for network messages and structures
- **Message Parsers**: Tools for parsing and processing network messages

## Transport System

Orion.Network.Core defines a transport system through the `INetworkTransport` interface, allowing different implementations to provide network connectivity:

```csharp
public interface INetworkTransport
{
    event ClientConnectedHandler ClientConnected;
    event ClientDisconnectedHandler ClientDisconnected;
    event MessageReceivedHandler MessageReceived;

    string Id { get; }
    string Name { get; }
    NetworkProtocolType Protocol { get; }
    NetworkSecurityType Security { get; }
    ServerNetworkType ServerNetworkType { get; }
    string IpAddress { get; }
    int Port { get; }

    Task StartAsync();
    Task StopAsync();
    Task SendAsync(string sessionId, byte[] message, CancellationToken cancellationToken = default);
    bool HaveSession(string sessionId);
}
```

## Examples

### Working with the Transport Manager

```csharp
using Orion.Network.Core.Interfaces.Services;
using Orion.Network.Core.Data;
using System.Text;

public class NetworkExample
{
    private readonly INetworkTransportManager _transportManager;

    public NetworkExample(INetworkTransportManager transportManager)
    {
        _transportManager = transportManager;

        // Subscribe to incoming messages
        _transportManager.IncomingMessages.Subscribe(HandleIncomingMessage);

        // Handle connection events
        _transportManager.ClientConnected += OnClientConnected;
        _transportManager.ClientDisconnected += OnClientDisconnected;
    }

    private void OnClientConnected(string transportId, string sessionId, string endpoint)
    {
        Console.WriteLine($"Client connected: {sessionId} from {endpoint} via {transportId}");
    }

    private void OnClientDisconnected(string transportId, string sessionId, string endpoint)
    {
        Console.WriteLine($"Client disconnected: {sessionId} from {endpoint} via {transportId}");
    }

    private async Task HandleIncomingMessage(NetworkMessageData message)
    {
        Console.WriteLine($"Message from {message.SessionId}: {message.Message}");

        // Send a response
        await _transportManager.EnqueueMessageAsync(
            new NetworkMessageData(message.SessionId, "Echo: " + message.Message, message.ServerNetworkType)
        );
    }
}
```

### Implementing a Custom Transport

```csharp
using Orion.Core.Types;
using Orion.Network.Core.Interfaces.Transports;
using Orion.Network.Core.Types;

public class MyCustomTransport : INetworkTransport
{
    public event INetworkTransport.ClientConnectedHandler ClientConnected;
    public event INetworkTransport.ClientDisconnectedHandler ClientDisconnected;
    public event INetworkTransport.MessageReceivedHandler MessageReceived;

    public string Id { get; } = Guid.NewGuid().ToString();
    public string Name { get; } = "MyCustomTransport";
    public NetworkProtocolType Protocol => NetworkProtocolType.Custom;
    public NetworkSecurityType Security => NetworkSecurityType.None;
    public ServerNetworkType ServerNetworkType { get; }
    public string IpAddress { get; }
    public int Port { get; }

    private readonly Dictionary<string, MyClient> _clients = new();

    public MyCustomTransport(ServerNetworkType serverNetworkType, string ipAddress, int port)
    {
        ServerNetworkType = serverNetworkType;
        IpAddress = ipAddress;
        Port = port;
    }

    public Task StartAsync()
    {
        // Initialize transport
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        // Shutdown transport
        return Task.CompletedTask;
    }

    public Task SendAsync(string sessionId, byte[] message, CancellationToken cancellationToken = default)
    {
        if (_clients.TryGetValue(sessionId, out var client))
        {
            return client.SendAsync(message);
        }

        throw new InvalidOperationException($"Session {sessionId} not found.");
    }

    public bool HaveSession(string sessionId)
    {
        return _clients.ContainsKey(sessionId);
    }

    // Helper class for the example
    private class MyClient
    {
        public Task SendAsync(byte[] message) => Task.CompletedTask;
    }
}
```

### Using the Message Parser

```csharp
using Orion.Network.Core.Parsers;

// Parse messages from byte buffer
byte[] buffer = Encoding.UTF8.GetBytes("NICK user1\r\nUSER user1 0 * :Real Name\r\n");
var messages = NewLineMessageParser.ParseMessages(buffer);

foreach (var message in messages)
{
    Console.WriteLine($"Parsed message: {message}");
}
```

## Dependencies

- **Orion.Core**: Core utilities and extensions
- **Orion.Irc.Core**: IRC protocol implementation
- **System.Reactive**: Reactive programming support
- **Microsoft.Extensions.Logging.Abstractions**: Logging infrastructure

## Related Packages

- **Orion.Network.Tcp**: TCP implementation for network transports
- **Orion.Core.Server**: Server-side core functionality

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Project Links

- GitHub: [https://github.com/tgiachi/orion](https://github.com/tgiachi/orion)
- NuGet: [https://www.nuget.org/packages/Orion.Network.Core](https://www.nuget.org/packages/Orion.Network.Core
