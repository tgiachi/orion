using Microsoft.AspNetCore.Mvc;
using Orion.Core.Server.Data.Rest;
using Orion.Core.Server.Interfaces.Services.System;

namespace Orion.Server.Routes;

public static class AuthRoutes
{
    public static IEndpointRouteBuilder MapAuth(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("auth").WithTags("Auth").WithDescription("Authentication routes");


        group.MapPost(
                "/login",
                async ([FromBody] LoginRequest loginRequest, IAuthService authService) =>
                {
                    var response = await authService.LoginAsync(loginRequest);

                    if (!response.IsSuccess)
                    {
                        return Results.BadRequest(response);
                    }

                    return Results.Ok(response);
                }
            )
            .WithDescription("Login to the server")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName("Login");

        return endpoints;
    }
}
