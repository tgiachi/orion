# Orion.Network.Tcp

![NuGet Version](https://img.shields.io/nuget/v/Orion.Network.Tcp)
![License](https://img.shields.io/badge/license-MIT-green)
![.NET](https://img.shields.io/badge/.NET-9.0-purple)

TCP implementation for the Orion IRC Server networking layer.

> IRC is not dead, long live IRC!

## About

Orion.Network.Tcp provides a concrete TCP implementation of the network transport abstractions defined in Orion.Network.Core. This library enables Orion IRC Server to communicate with clients over standard TCP connections, with support for both secure (SSL/TLS) and non-secure channels.

## Installation

```bash
dotnet add package Orion.Network.Tcp
```

Or using the Package Manager Console:

```
Install-Package Orion.Network.Tcp
```

## Key Features

- **TCP Transport**: Implementation of INetworkTransport for TCP communication
- **SSL/TLS Support**: Secure communication with SSL/TLS encryption
- **Session Management**: Tracking and managing TCP client sessions
- **Performance Optimized**: Efficient buffer handling and message parsing
- **Flexible Configuration**: Support for various TCP server configurations
- **NetCoreServer Integration**: Built on top of the high-performance NetCoreServer library

## Components

The library includes several key components:

- **NonSecureTcpServer**: TCP server implementation without encryption
- **SecureTcpServer**: TCP server with SSL/TLS encryption
- **NonSecureTcpSession**: Session handler for non-secure connections
- **SecureTcpSession**: Session handler for secure connections

## Examples

### Registering TCP Transports with the Transport Manager

```csharp
using System.Net;
using System.Security.Authentication;
using NetCoreServer;
using Orion.Core.Types;
using Orion.Network.Core.Interfaces.Services;
using Orion.Network.Tcp.Servers;

public class NetworkSetup
{
    private readonly INetworkTransportManager _transportManager;

    public NetworkSetup(INetworkTransportManager transportManager)
    {
        _transportManager = transportManager;
    }

    public async Task ConfigureNetworkAsync()
    {
        // Add a non-secure TCP server for client connections
        var server = new NonSecureTcpServer(
            ServerNetworkType.Clients,
            IPAddress.Parse("0.0.0.0"),
            6667
        );
        _transportManager.AddTransport(server);

        // Add a secure TCP server with SSL/TLS
        var certificate = X509Certificate2.CreateFromPemFile("server.crt", "server.key");
        var sslContext = new SslContext(SslProtocols.Tls13, certificate);

        var secureServer = new SecureTcpServer(
            ServerNetworkType.Clients,
            sslContext,
            IPAddress.Parse("0.0.0.0"),
            6697
        );
        _transportManager.AddTransport(secureServer);

        // Start all transports
        await _transportManager.StartAsync();
    }
}
```

### Sending Messages to Clients

```csharp
using System.Text;
using Orion.Network.Core.Interfaces.Services;

public class MessageSender
{
    private readonly INetworkTransportManager _transportManager;

    public MessageSender(INetworkTransportManager transportManager)
    {
        _transportManager = transportManager;
    }

    public async Task SendMessageAsync(string sessionId, string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message + "\r\n");

        // Check if session exists
        if (_transportManager.HasSession(sessionId))
        {
            // Send the message
            await _transportManager.SendAsync(sessionId, data);
        }
    }

    public async Task BroadcastAsync(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message + "\r\n");

        // Get all sessions and send to each
        var sessions = _transportManager.GetAllSessions();
        foreach (var sessionId in sessions)
        {
            await _transportManager.SendAsync(sessionId, data);
        }
    }
}
```

## Implementation Details

### NonSecureTcpServer

The `NonSecureTcpServer` class implements `INetworkTransport` to provide non-encrypted TCP connectivity:

```csharp
public class NonSecureTcpServer : TcpServer, INetworkTransport
{
    // INetworkTransport event handlers
    public event INetworkTransport.ClientConnectedHandler? ClientConnected;
    public event INetworkTransport.ClientDisconnectedHandler? ClientDisconnected;
    public event INetworkTransport.MessageReceivedHandler? MessageReceived;

    // Implementation details follow...
}
```

### SecureTcpServer

The `SecureTcpServer` class provides SSL/TLS-encrypted TCP connectivity:

```csharp
public class SecureTcpServer : SslServer, INetworkTransport
{
    // INetworkTransport event handlers
    public event INetworkTransport.ClientConnectedHandler? ClientConnected;
    public event INetworkTransport.ClientDisconnectedHandler? ClientDisconnected;
    public event INetworkTransport.MessageReceivedHandler? MessageReceived;

    // Implementation details follow...
}
```

## Dependencies

- **Orion.Core**: Core utilities and extensions
- **Orion.Network.Core**: Networking abstractions
- **NetCoreServer**: High-performance socket server library

## Related Packages

- **Orion.Core**: Core utilities and extensions
- **Orion.Core.Server**: Server-side core functionality
- **Orion.Irc.Core**: IRC protocol implementation
- **Orion.Network.Core**: Networking abstractions

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Project Links

- GitHub: [https://github.com/tgiachi/orion](https://github.com/tgiachi/orion)
- NuGet: [https://www.nuget.org/packages/Orion.Network.Tcp](https://www.nuget.org/packages/Orion.Network.Tcp)
