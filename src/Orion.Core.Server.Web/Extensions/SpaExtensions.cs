using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;

namespace Orion.Core.Server.Web.Extensions;

public static class SpaExtensions
{
    public static IApplicationBuilder UseOrionSpa(this IApplicationBuilder app, string webRootPath)
    {
        if (!Directory.Exists(webRootPath))
        {
            Directory.CreateDirectory(webRootPath);
        }


        app.UseStaticFiles(
            new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(webRootPath),
                RequestPath = ""
            }
        );


        app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == 404 &&
                    !context.Request.Path.Value.StartsWith("/api") &&
                    !Path.HasExtension(context.Request.Path.Value))
                {
                    context.Request.Path = "/index.html";
                    context.Response.StatusCode = 200;
                    await next();
                }
            }
        );

        return app;
    }
}
