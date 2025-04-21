# Orion.Foundations

![Version](https://img.shields.io/badge/version-0.4.0-blue)
![License](https://img.shields.io/badge/license-MIT-green)
![.NET](https://img.shields.io/badge/.NET-9.0-purple)

Orion.Foundations is the base library providing utilities, extensions, and core classes for all Orion IRC Server components.

## Overview

This library serves as the foundation for the entire Orion ecosystem, offering common functionality used across all modules. It includes tools for cryptographic operations, string manipulation, serialization, networking, and more.

## Key Components

### Extensions

Extensions provide additional methods to existing types to simplify common operations.

#### Base64MethodEx

```csharp
// Convert a string to Base64
string encoded = "Hello, World!".ToBase64();

// Convert from Base64 to string
string decoded = encoded.FromBase64();

// Check if a string is Base64 formatted
bool isBase64 = "SGVsbG8gV29ybGQh".IsBase64String();
```

#### ByteUtilsMethodEx

```csharp
// Calculate MD5 checksum of a byte array
byte[] data = Encoding.UTF8.GetBytes("test data");
string md5 = data.GetMd5Checksum();

// Calculate MD5 checksum of a string
string md5String = "test data".GetMd5Checksum();
```

#### DateTimeMethodEx

```csharp
// Get Unix timestamp
long timestamp = DateTime.UtcNow.ToUnixTimestamp();

// Convert from Unix timestamp to DateTime
DateTime date = timestamp.FromEpoch();

// Get milliseconds of a date
long mills = DateTime.UtcNow.GetMills();
```

#### EncryptExtensions

```csharp
// Encrypt a string with a Base64 key
string encrypted = "Sensitive data".EncryptString(base64Key);

// Decrypt
string decrypted = encrypted.DecryptString(base64Key);
```

#### EnvReplacerMethodEx

```csharp
// Replace environment variables in a string
string template = "Home directory: {HOME}";
string result = template.ReplaceEnvVariable();
```

#### IpAddressExtension

```csharp
// Convert a string to IPAddress
var ip = "127.0.0.1".ToIpAddress();

// Handle special cases
var any = "*".ToIpAddress(); // Returns IPAddress.Any (0.0.0.0)
var ipv6Any = "::".ToIpAddress(); // Returns IPAddress.IPv6Any (::)
```

#### JsonMethodExtension

```csharp
// Serialize an object to JSON
string json = myObject.ToJson();

// Deserialize from JSON
MyClass obj = json.FromJson<MyClass>();
```

#### StringMethodExtension

```csharp
string text = "HelloWorld";

// Various format conversions
string snake = text.ToSnakeCase();     // "hello_world"
string camel = text.ToCamelCase();     // "helloWorld"
string pascal = text.ToPascalCase();   // "HelloWorld"
string kebab = text.ToKebabCase();     // "hello-world"
string title = text.ToTitleCase();     // "Hello World"
string upper = text.ToSnakeCaseUpper(); // "HELLO_WORLD"
```

#### StringToPortsExtension

```csharp
// Convert a port string to a list of integers
string portRange = "6660-6669,6697";
IEnumerable<int> ports = portRange.ToPorts();
// Returns: 6660, 6661, 6662, 6663, 6664, 6665, 6666, 6667, 6668, 6669, 6697
```

#### YamlMethodExtension

```csharp
// Serialize to YAML
string yaml = myObject.ToYaml();

// Deserialize from YAML
MyConfig config = yaml.FromYaml<MyConfig>();
```

### Utilities

Utility classes that provide specialized functionality.

#### DnsUtils

```csharp
// Resolve a hostname from an IP address
var result = await DnsUtils.TryResolveHostnameAsync("192.168.1.1");
if (result.Resolved)
{
    Console.WriteLine($"Hostname: {result.HostName}");
}
```

#### HashUtils

```csharp
// Calculate SHA-256 hash
string hash = HashUtils.ComputeSha256Hash("password");

// Create secure password hash
string passwordHash = HashUtils.CreatePassword("myPassword");

// Verify password
bool isValid = HashUtils.VerifyPassword("myPassword", passwordHash);

// Generate refresh token
string token = HashUtils.GenerateRandomRefreshToken();

// Generate Base64 key
string key = HashUtils.GenerateBase64Key();

// AES encryption
byte[] encrypted = HashUtils.Encrypt("sensitive data", keyBytes);
string decrypted = HashUtils.Decrypt(encrypted, keyBytes);
```

#### HostMaskUtils

```csharp
// Check if a host mask pattern matches
bool isMatch = HostMaskUtils.IsHostMaskMatch("user@*.example.com", "nick!user@host.example.com");

// Parse a user mask
HostMaskUtils.ParseUserMask("nick!user@host", out string nick, out string user, out string host);
```

#### JsonUtils

```csharp
// Get default JSON settings
var options = JsonUtils.GetDefaultJsonSettings();

// Serialize with AOT support
string json = JsonUtils.Serialize(myObject, jsonTypeInfo);

// Deserialize with AOT support
MyClass obj = JsonUtils.Deserialize<MyClass>(json, jsonTypeInfo);
```

#### PortToListParserUtils

```csharp
// Parse a port range string
IEnumerable<int> ports = PortToListParserUtils.ParsePorts("6660-6669,6697");
```

#### ResourceUtils

```csharp
// Read an embedded resource
string resource = ResourceUtils.ReadEmbeddedResource("Assets.header.txt", assembly);
```

#### SSLUtils

```csharp
// Load an SSL certificate
var certificate = SSLUtils.LoadCertificate("certificate.pem", "password");
```

#### StringUtils

```csharp
// Various case conversions
string snake = StringUtils.ToSnakeCase("HelloWorld");
string camel = StringUtils.ToCamelCase("hello_world");
string pascal = StringUtils.ToPascalCase("hello_world");
string kebab = StringUtils.ToKebabCase("HelloWorld");
string title = StringUtils.ToTitleCase("hello_world");
string upper = StringUtils.ToUpperSnakeCase("helloWorld");
```

#### YamlUtils

```csharp
// Serialize to YAML
string yaml = YamlUtils.Serialize(myObject);

// Deserialize from YAML
MyConfig config = YamlUtils.Deserialize<MyConfig>(yaml);

// Deserialize with dynamic type
object obj = YamlUtils.Deserialize(yaml, typeof(MyType));
```

### Observable Pattern

Classes to support the Observable pattern with System.Reactive.

#### CancellationDisposable

```csharp
// Implement an IDisposable that cancels a CancellationTokenSource
var cts = new CancellationTokenSource();
IDisposable disposable = new CancellationDisposable(cts);
// When disposable.Dispose() is called, cts.Cancel() is executed
```

#### ChannelObservable

```csharp
// Convert a ChannelReader to IObservable
var channel = Channel.CreateUnbounded<string>();
var observable = new ChannelObservable<string>(channel.Reader);
observable.Subscribe(message => Console.WriteLine(message));
```

### Object Pooling

Implementation of an object pool for efficient resource reuse.

#### ObjectPool

```csharp
// Create a pool of reusable objects
var pool = new ObjectPool<MyDisposableClass>(initialSize: 10);

// Get an object from the pool
var obj = pool.Get();

// Use the object...

// Return the object to the pool when done
pool.Return(obj);

// Async version
var objAsync = await pool.GetAsync(cancellationToken);
```

## Enumerations and Types

Core type definitions used throughout the system.

#### LogLevelType

```csharp
// Standard log levels
public enum LogLevelType
{
    Trace,
    Debug,
    Information,
    Warning,
    Error
}
```

#### ServerNetworkType

```csharp
// Server network types
public enum ServerNetworkType
{
    None,
    Servers,
    Clients
}
```

#### ServiceLifetimeType

```csharp
// Service lifetime types for DI container
public enum ServiceLifetimeType
{
    Singleton,
    Scoped,
    Transient
}
```

## Usage in Orion

Orion.Foundations is used as the base for all other Orion components:

- **Orion.Core.Server**: Uses the utilities and extensions for configuration, logging, and event handling
- **Orion.Network.Core**: Leverages the network extensions and utilities for connection management
- **Orion.Network.Tcp**: Implements transports using the network utilities
- **Orion.Irc.Core**: Uses the string utilities and extensions for IRC command processing

## Conclusion

Orion.Foundations provides a wide range of utilities and tools that make developing Orion IRC Server more efficient and consistent. The library is designed to be easily extensible and reusable in various contexts.
