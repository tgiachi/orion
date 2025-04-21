using Orion.Core.Server.Interfaces.Services.System;

namespace Orion.Server.Routes;

public static class VariablesRoutes
{
    public static IEndpointRouteBuilder MapVariables(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("variables").WithTags("Variables").WithDescription("Variables routes");

        group.MapGet("/",
                (ITextTemplateService templateService) =>
                {
                    var variables = templateService.GetVariablesAndContent();
                    return Results.Ok(variables);
                }
            )
            .Produces<Dictionary<string, object>>()
            .WithDescription("Get all variables");



        return endpoints;


    }

}
