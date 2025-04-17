# Orion.Irc.Core

![NuGet Version](https://img.shields.io/nuget/v/Orion.Irc.Core)
![License](https://img.shields.io/badge/license-MIT-green)
![.NET](https://img.shields.io/badge/.NET-9.0-purple)

IRC protocol implementation for the Orion IRC Server project.

> IRC is not dead, long live IRC!

## About

Orion.Irc.Core provides a comprehensive implementation of the IRC protocol for building IRC servers and clients. This library handles parsing, formatting, and validating IRC messages according to RFC standards, offering a clean object-oriented approach to working with the protocol.

## Installation

```bash
dotnet add package Orion.Irc.Core
```

Or using the Package Manager Console:

```
Install-Package Orion.Irc.Core
```

## Key Features

- **Command System**: Complete implementation of IRC commands
- **Parser**: Efficient parsing of IRC messages
- **Command Builder**: Fluent builders for creating IRC commands
- **RFC Compliance**: Implementation follows IRC RFCs
- **Numeric Responses**: All standard IRC numeric responses
- **Error Handling**: Specialized error handling for IRC protocol issues
- **Type-Safe**: Strong typing for IRC commands and responses

## Supported Commands

Orion.Irc.Core implements numerous IRC commands including:

- Basic commands: JOIN, NICK, USER, QUIT, PRIVMSG, etc.
- Channel operations: MODE, KICK, INVITE, TOPIC, etc.
- Server operations: KILL, LINKS, OPER, etc.
- Information commands: WHO, WHOIS, ISON, etc.
- And many others!

## Examples

### Parsing IRC Messages

```csharp
using Orion.Irc.Core.Interfaces.Commands;
using Orion.Irc.Core.Interfaces.Parser;
using Orion.Irc.Core.Services;

// Create a parser
IIrcCommandParser parser = new IrcCommandParser(loggerFactory.CreateLogger<IrcCommandParser>());

// Register command handlers
parser.RegisterCommand<JoinCommand>();
parser.RegisterCommand<NickCommand>();
parser.RegisterCommand<UserCommand>();

// Parse a message
string message = ":nick!user@host JOIN #channel";
IIrcCommand command = await parser.ParseAsync(message);

// Cast to specific command type
if (command is JoinCommand joinCommand)
{
    Console.WriteLine($"User {joinCommand.Source} joined {joinCommand.Channels[0].ChannelName}");
}
```

### Creating and Serializing Commands

```csharp
using Orion.Irc.Core.Commands;
using Orion.Irc.Core.Data.Channels;

// Create a JOIN command
var joinCommand = new JoinCommand
{
    Channels = new List<JoinChannelData>
    {
        new JoinChannelData("#orion"),
        new JoinChannelData("#testing", "password")
    }
};

// Serialize to string
string message = joinCommand.Write();
// Result: "JOIN #orion,#testing password"

// Create a KICK command
var kickCommand = KickCommand.Create("server.name", "#channel", "baduser", "Violated rules");
// Serialize
string kickMessage = kickCommand.Write();
// Result: ":server.name KICK #channel baduser :Violated rules"
```

### Working with Numeric Responses

```csharp
using Orion.Irc.Core.Commands.Replies;

// Create a welcome message (RPL_WELCOME - 001)
var welcome = RplWelcome.Create(
    "irc.server.name",
    "nickname",
    "Welcome to the Orion IRC Network, nickname!user@host"
);

// Serialize
string welcomeMessage = welcome.Write();
// Result: ":irc.server.name 001 nickname :Welcome to the Orion IRC Network, nickname!user@host"
```

## Architecture

Orion.Irc.Core is designed with a clean, modular architecture:

- **Commands**: Classes representing IRC commands
- **Parser**: Transforms raw IRC messages to command objects
- **Interfaces**: Clean contracts for extensibility
- **Data Models**: Type-safe representation of IRC data

## Dependencies

- **Microsoft.Extensions.Logging.Abstractions**: Logging infrastructure

## Related Packages

- **Orion.Core**: Core utilities and extensions
- **Orion.Core.Server**: Server-side core functionality
- **Orion.Network.Core**: Networking abstractions

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Project Links

- GitHub: [https://github.com/tgiachi/orion](https://github.com/tgiachi/orion)
- NuGet: [https://www.nuget.org/packages/Orion.Irc.Core](https://www.nuget.org/packages/Orion.Irc.Core)
