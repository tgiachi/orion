using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Orion.Core.Extensions;
using Orion.Core.Server.Data.Config;
using Orion.Core.Server.Data.Config.Sections;
using Orion.Core.Server.Data.Options;
using Orion.Core.Server.Extensions;
using Orion.Core.Server.Web.Extensions;
using Orion.Network.Core.Modules.Container;
using Orion.Server.Modules.Container;
using Orion.Server.Routes;
using Scalar.AspNetCore;
using Serilog;

namespace Orion.Server;

public class Program
{
    private const string _openApiPath = "/openapi/v1/openapi.json";

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();

        var appContext = builder.Services.InitApplication<OrionServerOptions, OrionServerConfig>("Orion");

        builder.WebHost.ConfigureKestrelFromConfig(appContext.ServerConfig.WebHttp);
        builder.Services.ConfigureHttpJsonOptions(WebJsonExtension.ConfigureWebJson());


        Log.Logger = appContext.LoggerConfiguration.CreateLogger();

        builder.Logging.ClearProviders().AddSerilog();

        appContext.ServerOptions.ShowHeader(typeof(Program).Assembly);

        builder.Services
            .AddModule<DefaultServicesModule>()
            .AddModule<NetworkTransportModule>();


        var app = builder.Build();


        app.Map("/", () => Results.Text("Orion Server"));

        app.MapOpenApi();

        app.MapScalarApiReference(
            o =>
            {
                o.Title = "Orion Server";
                o.Theme = ScalarTheme.Kepler;
            }
        );


        var apiRoute = app.MapGroup("/v1/api");

        apiRoute.MapStatus();

        app.MapOpenApi(_openApiPath);


        app.Run();
    }
}
