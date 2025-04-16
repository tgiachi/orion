using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Orion.Core.Extensions;
using Orion.Core.Server.Data.Config;
using Orion.Core.Server.Data.Config.Sections;
using Orion.Core.Server.Data.Options;
using Orion.Core.Server.Extensions;
using Orion.Core.Server.Web.Extensions;
using Serilog;

namespace Orion.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder(args);

        var appContext = builder.Services.InitApplication<OrionServerOptions, OrionServerConfig>("Orion");

        builder.WebHost.ConfigureKestrelFromConfig(appContext.ServerConfig.WebHttp);
        builder.Services.ConfigureHttpJsonOptions(WebJsonExtension.ConfigureWebJson());


        Log.Logger = appContext.LoggerConfiguration.CreateLogger();

        builder.Logging.ClearProviders().AddSerilog();

        var app = builder.Build();


        app.Run();
    }
}
