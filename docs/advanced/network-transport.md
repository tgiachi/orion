# Network Transport Architecture

![Version](https://img.shields.io/badge/version-0.4.0-blue)
![License](https://img.shields.io/badge/license-MIT-green)
![.NET](https://img.shields.io/badge/.NET-9.0-purple)

## Overview

Orion IRC Server's network architecture is designed to be flexible, extensible, and high-performance. It uses a modular approach that abstracts the implementation details of network transports, allowing Orion to work with various communication mechanisms (TCP, WebSockets, etc.) through a unified interface.

## Core Components

The network architecture is built around several key components:

### INetworkTransport Interface

The `INetworkTransport` interface serves as the foundation of Orion's networking layer. It defines a contract that all network transport implementations must follow:

```csharp
public interface INetworkTransport
{
    event ClientConnectedHandler ClientConnected;
    event ClientDisconnectedHandler ClientDisconnected;
    event MessageReceivedHandler MessageReceived;

    string Id { get; }
    string Name { get; }
    int Port { get; }
    NetworkProtocolType Protocol { get; }
    NetworkSecurityType Security { get; }
    ServerNetworkType ServerNetworkType { get; }
    string IpAddress { get; }

    Task StartAsync();
    Task StopAsync();
    Task SendAsync(string sessionId, byte[] message, CancellationToken cancellationToken = default);
    bool HaveSession(string sessionId);
    Task DisconnectAsync(string sessionId);
}
```

This interface defines:
- Events for client connections, disconnections, and message reception
- Properties for identifying and describing the transport
- Methods for lifecycle management and message handling

### NetworkTransportManager

The `NetworkTransportManager` class is the central coordinator of network operations:

```csharp
public class NetworkTransportManager : INetworkTransportManager
{
    public event INetworkTransport.ClientConnectedHandler? ClientConnected;
    public event INetworkTransport.ClientDisconnectedHandler? ClientDisconnected;
    public List<NetworkTransportData> Transports { get; } = new();
    public IObservable<NetworkMetricData> NetworkMetrics => _networkMetricsSubject;
    public Channel<NetworkMessageData> IncomingMessages { get; }
    public Channel<NetworkMessageData> OutgoingMessages { get; }

    // Methods for managing transports and messages
}
```

The manager:
- Maintains a collection of registered transports
- Provides channels for incoming and outgoing messages
- Handles message routing between transports and the application
- Collects metrics on network performance
- Manages client sessions across all transports

### Transport Implementations

#### NonSecureTcpServer

The `NonSecureTcpServer` implements `INetworkTransport` for standard TCP connections:

```csharp
public class NonSecureTcpServer : TcpServer, INetworkTransport
{
    // Implementation of INetworkTransport for plain TCP connections
}
```

#### SecureTcpServer

The `SecureTcpServer` implements `INetworkTransport` for SSL/TLS encrypted TCP connections:

```csharp
public class SecureTcpServer : SslServer, INetworkTransport
{
    // Implementation of INetworkTransport for secured TCP connections
}
```

### Message Processing

The network layer uses a message parsing system to handle raw byte data:

```csharp
public static class NewLineMessageParser
{
    public static List<string> FastParseMessages(ReadOnlyMemory<byte> buffer)
    {
        // Parse raw byte data into IRC protocol messages
    }
}
```

### Network Service

The `NetworkService` class is responsible for setting up and configuring transport instances based on server configuration:

```csharp
public class NetworkService : INetworkService, IEventBusListener<ServerReadyEvent>
{
    // Handles initializing transports based on configuration
}
```

## Data Structures

### NetworkMessageData

Represents a message being sent or received:

```csharp
public struct NetworkMessageData(string sessionId, string message, ServerNetworkType serverNetworkType)
{
    public string SessionId { get; set; } = sessionId;
    public string Message { get; set; } = message;
    public ServerNetworkType ServerNetworkType { get; set; } = serverNetworkType;
}
```

### NetworkMetricData

Tracks network performance metrics:

```csharp
public class NetworkMetricData
{
    public string SessionId { get; set; }
    public string Endpoint { get; set; }
    public long BytesIn { get; set; }
    public long BytesOut { get; set; }
    public long PacketsIn { get; set; }
    public long PacketsOut { get; set; }

    // Methods for updating metrics
}
```

### NetworkTransportData

Wraps an `INetworkTransport` with additional metadata:

```csharp
public class NetworkTransportData
{
    public string Id { get; }
    public string Name { get; set; }
    public string IpAddress { get; set; }
    public int Port { get; set; }
    public INetworkTransport Transport { get; set; }
    public ServerNetworkType ServerNetworkType { get; set; }
}
```

## Message Flow

1. **Client Connection**:
  - Client establishes a connection to one of the transports
  - Transport raises `ClientConnected` event
  - `NetworkTransportManager` creates a session and notifies subscribers

2. **Receiving Messages**:
  - Client sends data to the transport
  - Transport processes the raw bytes and raises `MessageReceived` event
  - `NetworkTransportManager` parses the data into IRC messages
  - Messages are placed in `IncomingMessages` channel
  - Application processes the messages through command handlers

3. **Sending Messages**:
  - Application creates a message and identifies the target session
  - Message is placed in `OutgoingMessages` channel
  - `NetworkTransportManager` routes the message to the appropriate transport
  - Transport sends the message to the client

4. **Client Disconnection**:
  - Client disconnects or server initiates disconnection
  - Transport raises `ClientDisconnected` event
  - `NetworkTransportManager` cleans up the session and notifies subscribers

## Configuration

The networking layer is configured through the `NetworkConfig` section in the server configuration:

```yaml
network:
  certificate:
    certificate_path: path/to/certificate.pem
    password: certificate_password
  binds:
    - host: 0.0.0.0
      ports: 6660-6669,6697
      network_type: clients
      secure: false
      use_web_socket: false
    - host: 0.0.0.0
      ports: 6697
      network_type: clients
      secure: true
      use_web_socket: false
```

This configuration allows defining multiple network bindings with different:
- Host addresses
- Port ranges
- Network types (client or server connections)
- Security settings (plain or SSL/TLS)
- Protocol types (TCP or WebSocket)

## Benefits of the Architecture

1. **Modularity**: New transport types can be added without changing the core system
2. **Abstraction**: The IRC application logic doesn't need to know the transport details
3. **Scalability**: Multiple transports can run simultaneously on different ports/protocols
4. **Performance**: Optimized message handling with channel-based processing
5. **Metrics**: Built-in collection of performance data for monitoring
6. **Flexibility**: Support for different security levels and protocol types

## Future Extensions

The architecture is designed to allow future additions:
- WebSocket transport for browser-based clients
- UDP-based transports for efficient server-to-server communication
- Custom protocol transports for specialized clients

## Conclusion

Orion's network transport architecture provides a robust foundation for IRC communication while maintaining flexibility for future expansion. The clean separation between transport mechanisms and application logic makes the system easy to understand, maintain, and extend.
