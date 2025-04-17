# Orion IRC Server

![Version](https://img.shields.io/badge/version-0.1.0-blue)
![License](https://img.shields.io/badge/license-MIT-green)
![.NET](https://img.shields.io/badge/.NET-9.0-purple)

> IRC is not dead, long live IRC!

![Orion Logo](assets/orion_logo.png)
> Orion (Orione in italiano) is my cat and this project is dedicated to him.



[![Docker Image](https://img.shields.io/docker/v/tgiachi/orionircserver?label=docker&sort=date)](https://hub.docker.com/r/tgiachi/orionircserver)

Orion is a modern, scalable IRC server built with .NET 9.0, designed to provide robust functionality while maintaining performance and flexibility.

## 🌟 Features

- 🔒 Secure connections with SSL/TLS support
- 🌐 IPv4 and IPv6 support
- 🛠️ Completely modular and extensible architecture
- 📝 Scripting support via JavaScript engine (Jint)
- 📊 Performance metrics and diagnostics
- 📜 Text templating with Scriban
- 🧩 Plugin system
- 🐳 Docker support
- 🌉 Planning to create bridges for Discord, Matrix, and other platforms

## 📋 Project Structure

Orion is built as a collection of modular components:

- **Orion.Core**: Base utilities, extensions and common functionality
- **Orion.Core.Server**: Server-side core functionality
- **Orion.Core.Server.Web**: Web API and HTTP interface
- **Orion.Irc.Core**: IRC protocol implementation
- **Orion.Network.Core**: Networking abstractions
- **Orion.Network.Tcp**: TCP implementation for network transports
- **Orion.Server**: Main application entry point

## 🔧 Configuration

Orion uses YAML for configuration. On first run, a default configuration file is created at the specified path. Key configuration sections include:

```yaml
# Server identification
server:
  id: auto-generated
  host: irc.orion.io
  network: OrionNet
  description: Orion IRC server
  admin:
    name: Orion Admin
    email: admin@orion.io
    nickname: OrionAdmin

# Network settings
network:
  ssl:
    certificate_path: path/to/certificate
    password: certificate_password
  binds:
    - host: 0.0.0.0
      ports: 6660-6669,6670
      network_type: clients
      secure: false
      use_web_socket: false

# Web HTTP API
web_http:
  is_enabled: true
  listen_address: 0.0.0.0
  listen_port: 23021
  jwt_auth:
    issuer: Orion
    audience: Orion
    expiration_in_minutes: 44640  # 31 days
    refresh_token_expiry_days: 1
```

## 🚀 Getting Started

### Prerequisites

- .NET 9.0 SDK
- SSL certificate (optional, for secure connections)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/tgiachi/orion.git
   cd orion
   ```

### Docker

You can run Orion using Docker:

```bash
docker pull tgiachi/orionircserver
docker run -p 6667:6667 -p 23021:23021 tgiachi/orionircserver
```

2. Build the solution:
   ```bash
   dotnet build
   ```

3. Run the server:
   ```bash
   dotnet run --project src/Orion.Server
   ```

#### Command Line Options

```
-c, --config           Path to configuration file (default: orion.yml)
-r, --root-directory   Root directory for the server
-s, --show-header      Show header in console (default: true)
-l, --log-level        Log level (Trace, Debug, Information, Warning, Error)
-v, --verbose          Enable verbose output
```

## 📝 Scripting

Orion includes a JavaScript engine that allows you to extend functionality through scripts. Scripts are stored in the `scripts` directory and can use the modules registered with the server.

Example:

```javascript
// Access the logger module
logger.info("Server is running!");

// Define event handlers
function onStarted() {
    logger.info("Server has started successfully");
    // Custom initialization code here
}
```

The script engine automatically generates TypeScript definitions (`index.d.ts`) to provide code completion and documentation for available modules and functions.

## 👷 Development

### Adding New IRC Commands

To implement a new IRC command:

1. Create a class that inherits from `BaseIrcCommand`
2. Implement the `Parse` and `Write` methods
3. Register the command with the `IrcCommandParser` service

Example:

```csharp
public class MyCommand : BaseIrcCommand
{
    public MyCommand() : base("MYCMD") { }

    public override void Parse(string line)
    {
        // Parse command logic
    }

    public override string Write()
    {
        // Serialize command logic
        return $"MYCMD :parameters";
    }
}
```

### Creating Command Handlers

To handle IRC commands:

1. Create a class that inherits from `BaseIrcCommandListener`
2. Implement the `IIrcCommandHandler<T>` interface
3. Register the handler for specific server network types

Example:

```csharp
public class MyCommandHandler : BaseIrcCommandListener, IIrcCommandHandler<MyCommand>
{
    public MyCommandHandler(
        ILogger<BaseIrcCommandListener> logger,
        IIrcCommandService ircCommandService,
        IHyperPostmanService postmanService,
        IIrcSessionService sessionService
    ) : base(logger, ircCommandService, postmanService, sessionService)
    {
        RegisterCommandHandler<MyCommand>(this, ServerNetworkType.Clients);
    }

    public async Task OnCommandReceivedAsync(
        IrcUserSession session,
        ServerNetworkType serverNetworkType,
        MyCommand command
    )
    {
        // Handle the command
    }
}
```



## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 📬 Contact

Project Link: [https://github.com/tgiachi/orion](https://github.com/tgiachi/orion)
