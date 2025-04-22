# IRC Protocol Implementation

![Version](https://img.shields.io/badge/version-0.4.0-blue)
![License](https://img.shields.io/badge/license-MIT-green)
![.NET](https://img.shields.io/badge/.NET-9.0-purple)

## Overview

Orion implements the Internet Relay Chat (IRC) protocol through a clean, object-oriented design centered around command processing. The implementation follows RFC standards while providing a modern extensibility model for adding custom functionality.

## Core Components

### BaseIrcCommand

The foundation of the IRC command system:

```csharp
public abstract class BaseIrcCommand : IIrcCommand
{
    private readonly string _code;

    public string Code => _code;

    public virtual void Parse(string line) { }
    public virtual string Write() { return string.Empty; }

    protected BaseIrcCommand(string code)
    {
        _code = code;
    }
}
```

### IIrcCommand Interface

The contract all IRC commands must implement:

```csharp
public interface IIrcCommand
{
    string Code { get; }
    void Parse(string line);
    string Write();
}
```

### IIrcCommandParser

Responsible for parsing and serializing IRC commands:

```csharp
public interface IIrcCommandParser
{
    Task<IIrcCommand> ParseAsync(string message);
    Task<string> SerializeAsync(IIrcCommand command);
    void RegisterCommand(IIrcCommand command);
}
```

### Command Handlers

Handlers that process specific IRC commands:

```csharp
public interface IIrcCommandHandler<in TCommand> where TCommand : IIrcCommand
{
    Task OnCommandReceivedAsync(IrcUserSession session, ServerNetworkType serverNetworkType, TCommand command);
}
```

## Command Types

Orion implements a comprehensive set of IRC commands:

### User Registration Commands

```csharp
public class NickCommand : BaseIrcCommand
{
    public string Nickname { get; set; }
    public string Source { get; set; }

    // Implementation details
}

public class UserCommand : BaseIrcCommand
{
    public string UserName { get; set; }
    public string RealName { get; set; }
    public string Mode { get; set; }
    public string Unused { get; set; }

    // Implementation details
}

public class PassCommand : BaseIrcCommand
{
    public string Password { get; set; }

    // Implementation details
}
```

### Server Management Commands

```csharp
public class PingCommand : BaseIrcCommand
{
    public string Token { get; set; }
    public string Server { get; set; }

    // Implementation details
}

public class PongCommand : BaseIrcCommand
{
    public string Token { get; set; }
    public string Server { get; set; }

    // Implementation details
}

public class AdminCommand : BaseIrcCommand
{
    public string Source { get; set; }
    public string Target { get; set; }

    // Implementation details
}
```

### Channel Operations Commands

```csharp
public class JoinCommand : BaseIrcCommand
{
    public string Source { get; set; }
    public string Channels { get; set; }
    public string Keys { get; set; }

    // Implementation details
}

public class PartCommand : BaseIrcCommand
{
    public string Source { get; set; }
    public string Channels { get; set; }
    public string Message { get; set; }

    // Implementation details
}
```

### Communication Commands

```csharp
public class PrivmsgCommand : BaseIrcCommand
{
    public string Source { get; set; }
    public string Target { get; set; }
    public string Message { get; set; }

    // Implementation details
}

public class NoticeCommand : BaseIrcCommand
{
    public string Source { get; set; }
    public string Target { get; set; }
    public string Message { get; set; }

    // Implementation details
}
```

### Server Response Codes

Orion implements numeric responses according to the IRC protocol:

```csharp
public class RplWelcome : BaseIrcCommand // 001
{
    public string ServerName { get; set; }
    public string Nickname { get; set; }
    public string Message { get; set; }

    // Implementation details
}

public class ErrNicknameInUse : BaseIrcCommand // 433
{
    public string ServerName { get; set; }
    public string Nickname { get; set; }
    public string Message { get; set; }

    // Implementation details
}

public class RplAdminEmail : BaseIrcCommand // 259
{
    public string Nickname { get; set; }
    public string ServerName { get; set; }
    public string EmailAddress { get; set; }

    // Implementation details
}
```

## Message Flow

1. **Message Reception**:
  - Network transport receives raw message data
  - Message is passed to `IrcCommandParser`

2. **Command Parsing**:
  - Parser identifies the command code (e.g., "NICK", "USER")
  - Creates appropriate command object
  - Calls `Parse` method to extract parameters

3. **Command Handling**:
  - Command is dispatched to registered handlers
  - Handler processes the command and performs necessary actions
  - May generate response commands

4. **Response Generation**:
  - Response commands created by handlers
  - `Write` method serializes them to strings
  - Sent back to client via network transport

## Example: Client Registration Process

The IRC client registration process illustrates the command flow:

1. Client sends `NICK` command:
   ```
   NICK mario
   ```

2. `NickCommand` object is created and parsed:
   ```csharp
   var nickCommand = new NickCommand { Nickname = "mario" };
   ```

3. `ConnectionHandler` processes the command:
   ```csharp
   public async Task OnCommandReceivedAsync(
       IrcUserSession session, ServerNetworkType serverNetworkType, NickCommand command)
   {
       var exists = QuerySessions(s => s.NickName.Equals(command.Nickname, StringComparison.OrdinalIgnoreCase));

       if (exists.Count == 0)
       {
           session.NickName = command.Nickname;
       }
       else
       {
           await session.SendCommandAsync(
               ErrNicknameInUse.CreateForUnregistered(ServerContextData.ServerName, command.Nickname)
           );
       }

       await SendIfAuthenticated(session);
   }
   ```

4. Client sends `USER` command:
   ```
   USER mario 0 * :Mario Rossi
   ```

5. `UserCommand` object is created and parsed:
   ```csharp
   var userCommand = new UserCommand
   {
       UserName = "mario",
       Mode = "0",
       Unused = "*",
       RealName = "Mario Rossi"
   };
   ```

6. `ConnectionHandler` processes the command:
   ```csharp
   public async Task OnCommandReceivedAsync(
       IrcUserSession session, ServerNetworkType serverNetworkType, UserCommand command)
   {
       session.RealName = command.RealName;
       session.UserName = command.UserName;

       await SendIfAuthenticated(session);
   }
   ```

7. If both NICK and USER are received, server sends welcome sequence:
   ```csharp
   private async Task SendIfAuthenticated(IrcUserSession session)
   {
       if (session.IsAuthenticated)
       {
           await PublishEventAsync(new UserAuthenticatedEvent(session.SessionId));

           // Send welcome messages, server info, etc.
           await session.SendCommandAsync(
               RplWelcome.Create(ServerContextData.ServerName, session.NickName, "Welcome to the OrionIRC Network")
           );
           // Additional welcome messages...
       }
   }
   ```

## Registration Process Flow

```
Client                                    Server
  |                                         |
  |--- NICK mario ------------------------->|
  |                                         |
  |<-- :irc.orion.io NOTICE * :Checking -->|
  |<-- :irc.orion.io NOTICE * :Looking  -->|
  |                                         |
  |--- USER mario 0 * :Mario Rossi -------->|
  |                                         |
  |<-- :irc.orion.io 001 mario :Welcome --->|
  |<-- :irc.orion.io 002 mario :Your host ->|
  |<-- :irc.orion.io 003 mario :This... --->|
  |<-- :irc.orion.io 004 mario :irc... ----->|
  |                                         |
```

## Extending the Protocol

### Adding New Commands

To add a new IRC command:

1. Create a class that inherits from `BaseIrcCommand`:
   ```csharp
   public class MyCommand : BaseIrcCommand
   {
       public MyCommand() : base("MYCMD") { }

       public string Parameter1 { get; set; }
       public string Parameter2 { get; set; }

       public override void Parse(string line)
       {
           // Parsing logic
       }

       public override string Write()
       {
           // Serialization logic
           return $"MYCMD {Parameter1} {Parameter2}";
       }
   }
   ```

2. Register the command with the parser:
   ```csharp
   services.AddIrcCommand<MyCommand>();
   ```

3. Create a handler for the command:
   ```csharp
   public class MyCommandHandler : BaseIrcCommandListener, IIrcCommandHandler<MyCommand>
   {
       public MyCommandHandler(ILogger<MyCommandHandler> logger, IrcCommandListenerContext context)
           : base(logger, context)
       {
           RegisterCommandHandler<MyCommand>(this, ServerNetworkType.Clients);
       }

       public async Task OnCommandReceivedAsync(
           IrcUserSession session, ServerNetworkType serverNetworkType, MyCommand command)
       {
           // Command handling logic
       }
   }
   ```

### Text Formatting

Orion supports mIRC-style text formatting:

```csharp
public static class MircNotationConverter
{
    public static string Convert(string input)
    {
        // Converts text formatting tags like [BOLD] to IRC control codes
    }

    public static string ConvertBack(string input)
    {
        // Converts IRC control codes back to text formatting tags
    }
}
```

## Client-Server Communication Examples

### Connection Establishment

```
Client: CAP LS 302
Server: :irc.orion.io NOTICE * :Checking for clones
Server: :irc.orion.io NOTICE * :Looking up hostname
Server: :irc.orion.io NOTICE * :Connecting to host example.com
```

### Authentication

```
Client: PASS secretpassword
Client: NICK mario
Client: USER mario 0 * :Mario Rossi
```

### Channel Operations

```
Client: JOIN #orion
Server: :mario!mario@example.com JOIN #orion
Server: :irc.orion.io 353 mario = #orion :mario @admin
Server: :irc.orion.io 366 mario #orion :End of /NAMES list
```

### Messaging

```
Client: PRIVMSG #orion :Hello, world!
Server: :mario!mario@example.com PRIVMSG #orion :Hello, world!
```

### Keep-Alive

```
Server: PING :1618426935000
Client: PONG :1618426935000
```

## Performance Considerations

- Command objects are pooled when possible to reduce GC pressure
- Parsing is optimized for minimal string allocations
- Asynchronous processing ensures server responsiveness
- Event-based architecture allows scaling to many clients

## Conclusion

Orion's IRC protocol implementation provides a comprehensive, object-oriented approach to the IRC protocol. The command-based architecture makes it easy to add, modify, and extend functionality while maintaining compatibility with existing IRC clients.
