# Orion IRC Server Documentation

Welcome to the Orion IRC Server documentation.

## Getting Started

Orion IRC Server is a modern, scalable IRC server built with .NET 9.0, designed to provide robust functionality while maintaining performance and flexibility.

### Installation

You can run Orion IRC Server using Docker:

```bash
docker pull tgiachi/orionirc-server:latest
docker run -p 6667:6667 -p 23021:23021 tgiachi/orionirc-server:latest
```

Or you can build and run it from source:

```bash
git clone https://github.com/tgiachi/orion.git
cd orion
dotnet build
dotnet run --project src/Orion.Server
```

### Configuration

Configuration documentation and examples are available in the [articles section](articles/configuration.md).

## Core Components

Orion is built as a collection of modular components:

- **Orion.Core**: Base utilities, extensions and common functionality
- **Orion.Core.Server**: Server-side core functionality
- **Orion.Core.Server.Web**: Web API and HTTP interface
- **Orion.Irc.Core**: IRC protocol implementation
- **Orion.Network.Core**: Networking abstractions
- **Orion.Network.Tcp**: TCP implementation for network transports
- **Orion.Server**: Main application entry point

## API Documentation

Explore the [API documentation](api/index.html) for detailed information about the server components.

## Scripting

Orion includes a JavaScript engine that allows you to extend functionality through scripts. Learn more in the [scripting guide](articles/scripting.md).

## Configuration

Orion uses YAML for configuration. See the [configuration reference](articles/configuration.md) for detailed information.

## Network Structure

Learn about networking in Orion in the [network guide](articles/networking.md).

## Command Reference

Explore supported IRC commands in the [command reference](articles/commands.md).

## Bridges

Information about the planned bridges to other platforms (Discord, Matrix, etc.) is available in the [bridges documentation](articles/bridges.md).
