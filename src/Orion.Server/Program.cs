using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using Orion.Core.Irc.Server.Data.Internal;
using Orion.Core.Irc.Server.Extensions;
using Orion.Core.Server.Data.Config;
using Orion.Core.Server.Data.Config.Sections;
using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Data.Options;
using Orion.Core.Server.Extensions;
using Orion.Core.Server.Modules.Container;
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

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = appContext.Config.WebHttp.JwtAuth.Issuer,
                        ValidAudience = appContext.Config.WebHttp.JwtAuth.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(appContext.Config.WebHttp.JwtAuth.Secret)
                        )
                    };
                }
            );
        builder.Services.AddAuthorization();


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


        builder.Services.AddModule<DefaultOrionServiceModule>();

        builder.Services.AddModule<DefaultOrionScriptsModule>();

        builder.Services
            .AddEventBusService()
            .AddProcessQueueService()
            .AddScriptEngineService();

        builder.Services.AddSingleton<IrcCommandListenerContext>();

        builder.Services.CreateIrcServerAppContext(appContext.Config);

        builder.Services
            .AddModule<IrcCommandModule>()
            .AddModule<DefaultIrcListenerModule>();

        builder.Services.AddHostedService<OrionHostedService>();

        builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowLocalhost",
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    }
                );
            }
        );

        var app = builder.Build();


        app.UseCors("AllowLocalhost");

        app.Map("/", () => Results.Redirect("/index.html"));

        app.MapOpenApi();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapScalarApiReference(o =>
            {
                o.Title = "Orion Server";
                o.Theme = ScalarTheme.Kepler;
                o.Authentication = new ScalarAuthenticationOptions()
                {
                    PreferredSecurityScheme = "Bearer",
                    Http = new HttpOptions()
                    {
                    }
                };
            }
        );


        var apiRoute = app.MapGroup("/api/v1");

        apiRoute
            .MapSystem()
            .MapVariables()
            .MapAuth()
            .MapChannels()
            .MapStatus();

        app.MapOpenApi(_openApiPath);

        //Get current Directory


        var webRootPath = Path.Combine(Environment.CurrentDirectory, "Assets", "Web");


        app.UseOrionSpa(webRootPath);

        app.Run();
    }
}
