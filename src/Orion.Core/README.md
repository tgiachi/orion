# Orion.Core

![NuGet Version](https://img.shields.io/nuget/v/Orion.Core)
![License](https://img.shields.io/badge/license-MIT-green)
![.NET](https://img.shields.io/badge/.NET-9.0-purple)

Core utilities and extensions for the Orion IRC Server project.

> IRC is not dead, long live IRC!

## About

Orion.Core provides essential utilities, extensions, and base classes for building IRC server components. This library forms the foundation of the Orion IRC Server ecosystem, offering common functionality that can be used across various Orion modules or in your own IRC-related projects.

## Installation

```bash
dotnet add package Orion.Core
```

Or using the Package Manager Console:

```
Install-Package Orion.Core
```

## Key Features

- **Base64 Extensions**: Handle Base64 encoding and decoding
- **ByteUtils**: Utilities for MD5 checksum operations
- **DateTime Extensions**: Unix timestamp operations, epoch conversion
- **Encryption Extensions**: String encryption/decryption
- **Environment Variable Replacement**: Replace environment variables in strings
- **Hash Utilities**: Cryptographic operations, password handling, AES encryption
- **Host Mask Utilities**: IRC host mask operations
- **IP Address Extensions**: IP address handling
- **JSON Utilities**: Serialization and deserialization with AOT support
- **String Utilities**: Various string manipulations, including case conversions
- **YAML Utilities**: YAML serialization and deserialization

## Examples

### Base64 Operations

```csharp
using Orion.Core.Extensions;

// Convert string to Base64
string encoded = "Hello, World!".ToBase64();

// Convert Base64 to string
string decoded = encoded.FromBase64();

// Check if a string is Base64
bool isBase64 = encoded.IsBase64String();
```

### DateTime Operations

```csharp
using Orion.Core.Extensions;

// Get Unix timestamp
long timestamp = DateTime.UtcNow.ToUnixTimestamp();

// Convert from Unix timestamp
DateTime date = timestamp.FromEpoch();
```

### String Case Conversions

```csharp
using Orion.Core.Extensions;

string text = "HelloWorld";

string snake = text.ToSnakeCase();     // "hello_world"
string camel = text.ToCamelCase();     // "helloWorld"
string pascal = text.ToPascalCase();   // "HelloWorld"
string kebab = text.ToKebabCase();     // "hello-world"
string title = text.ToTitleCase();     // "Hello World"
```

### Environment Variable Replacement

```csharp
using Orion.Core.Extensions;

string template = "The home directory is {HOME}";
string result = template.ReplaceEnvVariable();
```

## Related Packages

- **Orion.Core.Server**: Server-side core functionality
- **Orion.Irc.Core**: IRC protocol implementation
- **Orion.Network.Core**: Networking abstractions

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Project Links

- GitHub: [https://github.com/tgiachi/orion](https://github.com/tgiachi/orion)
- NuGet: [https://www.nuget.org/packages/Orion.Core](https://www.nuget.org/packages/Orion.Core)
