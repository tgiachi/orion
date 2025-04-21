using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Orion.Core.Server.Data.Config;
using Orion.Core.Server.Data.Config.Sections;
using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Data.Options;
using Orion.Core.Server.Extensions;
using Orion.Core.Server.Web.Extensions;
using Orion.Server.Hosted;
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


        builder.Services.AddSingleton(appContext);

        builder.WebHost.ConfigureKestrelFromConfig(appContext.Config.WebHttp);
        builder.Services.ConfigureHttpJsonOptions(WebJsonExtension.ConfigureWebJson());


        Log.Logger = appContext.LoggerConfiguration.CreateLogger();

        builder.Logging.ClearProviders().AddSerilog();

        appContext.Options.ShowHeader(typeof(Program).Assembly);


        if (appContext.Config.Irc.Opers.Entries.Count == 0)
        {
            var defaultOper = new OperEntryConfig()
            {
                NickName = "admin",
                Host = "*@*",
                VHost = "opers.orion.io",
            };

            defaultOper.SetPassword("password");


            appContext.Config.Irc.Opers.Entries.Add(defaultOper);

            Log.Logger.Information(
                "Default oper entry created. Nick: {Nick}, Password: {Password} (and can use Api)",
                defaultOper.NickName,
                "password"
            );
        }

        appContext.Config.SaveConfig(appContext.ConfigFilePath);


        builder.Services
            .AddModule<DefaultServicesModule>()
            .AddModule<NetworkTransportModule>();


        builder.Services.AddModule<DefaultScriptsModule>();

        builder.Services.AddSingleton<IrcCommandListenerContext>();

        builder.Services.CreateIrcServerAppContext(appContext.Config);

        builder.Services
            .AddModule<IrcCommandModule>()
            .AddModule<DefaultIrcListenerModule>();

        builder.Services.AddHostedService<OrionHostedService>();

        var app = builder.Build();


        app.Map("/", () => Results.Text("Orion Server"));

        app.MapOpenApi();

        app.MapScalarApiReference(o =>
            {
                o.Title = "Orion Server";
                o.Theme = ScalarTheme.Kepler;
            }
        );


        var apiRoute = app.MapGroup("/api/v1");

        apiRoute
            .MapSystem()
            .MapVariables()
            .MapStatus();

        app.MapOpenApi(_openApiPath);

        app.Run();
    }
}
