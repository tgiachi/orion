# Orion.Core.Server.Web

![NuGet Version](https://img.shields.io/nuget/v/Orion.Core.Server.Web)
![License](https://img.shields.io/badge/license-MIT-green)
![.NET](https://img.shields.io/badge/.NET-9.0-purple)

Web API and HTTP interface for the Orion IRC Server project.

> IRC is not dead, long live IRC!

## About

Orion.Core.Server.Web provides the web API and HTTP interface for the Orion IRC Server. This library allows you to add a web-based management interface, REST APIs, and other HTTP-based functionality to your Orion IRC Server implementation.

## Installation

```bash
dotnet add package Orion.Core.Server.Web
```

Or using the Package Manager Console:

```
Install-Package Orion.Core.Server.Web
```

## Key Features

- **Web API**: REST API for server management and monitoring
- **ASP.NET Core Integration**: Leverage the power of ASP.NET Core
- **Kestrel Configuration**: Configure Kestrel web server from Orion's configuration
- **JSON Serialization**: Consistent JSON serialization across HTTP interfaces
- **Authentication**: JWT-based authentication for API access
- **Configuration-based Setup**: Enable/disable and configure web functionality via YAML config

## Examples

### Configuring Kestrel with Orion Settings

```csharp
using Microsoft.AspNetCore.Builder;
using Orion.Core.Server.Data.Config.Sections;
using Orion.Core.Server.Web.Extensions;

public static void ConfigureWeb(WebApplicationBuilder builder, WebHttpConfig webConfig)
{
    builder.WebHost.ConfigureKestrelFromConfig(webConfig);

    // Additional web configuration
}
```

### Setting up JSON for Web APIs

```csharp
using Microsoft.AspNetCore.Builder;
using Orion.Core.Server.Web.Extensions;

public static void ConfigureJson(WebApplicationBuilder builder)
{
    builder.Services.ConfigureHttpJsonOptions(WebJsonExtension.ConfigureWebJson());

    // Your application's JSON is now configured with Orion settings
}
```

### Creating a Basic API Endpoint

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

public static IEndpointRouteBuilder MapMyApi(this IEndpointRouteBuilder endpoints)
{
    var group = endpoints.MapGroup("api/myfeature")
        .WithTags("MyFeature")
        .WithDescription("My feature APIs");

    group.MapGet("/status", () => Results.Ok(new { Status = "Running" }))
        .WithName("GetStatus")
        .WithDescription("Get the status of my feature");

    return endpoints;
}
```

## Web Configuration

Example YAML configuration for the web interface:

```yaml
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

## Dependencies

- **Orion.Core**: Core utilities and extensions
- **Orion.Core.Server**: Server-side core functionality
- **Microsoft.AspNetCore.App**: ASP.NET Core framework

## Related Packages

- **Orion.Core**: Core utilities and extensions
- **Orion.Core.Server**: Server-side core functionality
- **Orion.Irc.Core**: IRC protocol implementation
- **Orion.Network.Core**: Networking abstractions

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Project Links

- GitHub: [https://github.com/tgiachi/orion](https://github.com/tgiachi/orion)
- NuGet: [https://www.nuget.org/packages/Orion.Core.Server.Web](https://www.nuget.org/packages/Orion.Core.Server.Web)
