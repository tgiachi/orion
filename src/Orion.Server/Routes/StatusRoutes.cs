namespace Orion.Server.Routes;

public static class StatusRoutes
{
    public static IEndpointRouteBuilder MapStatus(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("status").WithTags("Status").WithDescription("Status routes");

        group.MapGet("/health", () => Results.Ok())
            .WithDescription("Get the health status");

        return endpoints;
    }
}
