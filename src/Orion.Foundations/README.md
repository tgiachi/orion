# Orion.Foundations

![NuGet Version](https://img.shields.io/badge/version-0.6.0-blue)
![License](https://img.shields.io/badge/license-MIT-green)
![.NET](https://img.shields.io/badge/.NET-9.0-purple)

Core utilities and extensions for the Orion IRC Server project.

> IRC is not dead, long live IRC!

## About

Orion.Foundations provides essential utilities, extensions, and base classes for building IRC server components. This library forms the foundation of the Orion IRC Server ecosystem, offering common functionality that can be used across various Orion modules or in your own IRC-related projects.

## Installation

```bash
dotnet add package Orion.Foundations
```

Or using the Package Manager Console:

```
Install-Package Orion.Foundations
```

## Key Features

- **Base64 Extensions**: Handle Base64 encoding and decoding
- **ByteUtils**: Utilities for MD5 checksum operations
- **DateTime Extensions**: Unix timestamp operations, epoch conversion
- **Encryption Extensions**: String encryption/decryption
- **Environment Variable Replacement**: Replace environment variables in strings with pattern `{ENV_VAR}`
- **Hash Utilities**: Cryptographic operations, password handling, AES encryption
- **Host Mask Utilities**: IRC host mask operations (nick!user@host patterns)
- **IP Address Extensions**: IP address handling and conversion
- **JSON Utilities**: Serialization and deserialization with AOT support
- **String Utilities**: Various string manipulations, including case conversions
- **YAML Utilities**: YAML serialization and deserialization
- **DNS Utilities**: Hostname resolution helpers
- **Port Parser**: Parse port ranges like "6660-6669,6697"
- **Resource Utils**: Read embedded resources from assemblies
- **Observable Pattern**: Helpers for working with Reactive Extensions
- **Object Pool**: Generic object pooling

## Examples

### Base64 Operations

```csharp
using Orion.Foundations.Extensions;

// Convert string to Base64
string encoded = "Hello, World!".ToBase64();

// Convert Base64 to string
string decoded = encoded.FromBase64();

// Check if a string is Base64
bool isBase64 = encoded.IsBase64String();
```

### DateTime Operations

```csharp
using Orion.Foundations.Extensions;

// Get Unix timestamp
long timestamp = DateTime.UtcNow.ToUnixTimestamp();

// Convert from Unix timestamp
DateTime date = timestamp.FromEpoch();
```

### String Case Conversions

```csharp
using Orion.Foundations.Extensions;

string text = "HelloWorld";

string snake = text.ToSnakeCase();     // "hello_world"
string camel = text.ToCamelCase();     // "helloWorld"
string pascal = text.ToPascalCase();   // "HelloWorld"
string kebab = text.ToKebabCase();     // "hello-world"
string title = text.ToTitleCase();     // "Hello World"
string upper = text.ToSnakeCaseUpper(); // "HELLO_WORLD"
```

### Environment Variable Replacement

```csharp
using Orion.Foundations.Extensions;

string template = "The home directory is {HOME}";
string result = template.ReplaceEnvVariable();
```

### Hash and Encryption

```csharp
using Orion.Foundations.Utils;

// Create password hash
string hashedPassword = HashUtils.CreatePassword("myPassword");

// Verify password
bool isValid = HashUtils.VerifyPassword("myPassword", hashedPassword);

// Generate secure key
string key = HashUtils.GenerateBase64Key();

// AES encryption
byte[] encrypted = HashUtils.Encrypt("sensitive data", key.FromBase64ToByteArray());
string decrypted = HashUtils.Decrypt(encrypted, key.FromBase64ToByteArray());
```

### Port Range Parsing

```csharp
using Orion.Foundations.Extensions;

string portRange = "6660-6669,6697";
IEnumerable<int> ports = portRange.ToPorts();
// Returns: 6660, 6661, 6662, 6663, 6664, 6665, 6666, 6667, 6668, 6669, 6697
```

### IP Address Handling

```csharp
using Orion.Foundations.Extensions;

// Convert special patterns
var anyAddress = "*".ToIpAddress(); // Returns IPAddress.Any (0.0.0.0)
var ipv6Any = "::".ToIpAddress();   // Returns IPAddress.IPv6Any (::)

// Normal conversion
var loopback = "127.0.0.1".ToIpAddress();
```

### DNS Resolution

```csharp
using Orion.Foundations.Utils;

var result = await DnsUtils.TryResolveHostnameAsync("192.168.1.1");
if (result.Resolved)
{
    Console.WriteLine($"Hostname: {result.HostName}");
}
```

### YAML Serialization

```csharp
using Orion.Foundations.Extensions;

// Serialize to YAML
var config = new MyConfig();
string yaml = config.ToYaml();

// Deserialize from YAML
var loadedConfig = yaml.FromYaml<MyConfig>();
```

### Object Pooling

```csharp
using Orion.Foundations.Pool;

// Create a pool of reusable objects
var pool = new ObjectPool<MyDisposableClass>();

// Get an object from the pool
var obj = pool.Get();

// Use the object...

// Return it to the pool when done
pool.Return(obj);
```

## Project Structure

- **Extensions/**: Extension methods for various types
- **Utils/**: Utility classes and helper methods
- **Types/**: Enums and basic type definitions
- **Observable/**: Helpers for working with Reactive Extensions
- **Pool/**: Object pooling implementation

## Dependencies

- **YamlDotNet**: For YAML serialization and deserialization

## Related Packages

- **Orion.Core.Server**: Server-side core functionality
- **Orion.Irc.Core**: IRC protocol implementation
- **Orion.Network.Core**: Networking abstractions

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Project Links

- GitHub: [https://github.com/tgiachi/orion](https://github.com/tgiachi/orion)
- NuGet: [https://www.nuget.org/packages/Orion.Foundations](https://www.nuget.org/packages/Orion.Foundations)
