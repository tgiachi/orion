using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Interfaces.Services.System;

namespace Orion.Server.Routes;

public static class SystemRoutes
{
    public static IEndpointRouteBuilder MapSystem(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("system").WithTags("System").WithDescription("System routes");

        group.MapGet("/version",
                (IVersionService versionService) => Results.Ok((object?)versionService.GetVersionInfo())
            )
            .Produces<VersionInfoData>()
            .WithDescription("Get the version information");

        return endpoints;
    }
}
