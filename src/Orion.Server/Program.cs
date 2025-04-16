using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Orion.Core.Extensions;
using Orion.Core.Server.Data.Config;
using Orion.Core.Server.Data.Config.Sections;
using Orion.Core.Server.Data.Options;
using Orion.Core.Server.Extensions;
using Serilog;

namespace Orion.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder(args);


        var appContext = builder.Services.InitApplication<OrionServerOptions, OrionServerConfig>("Orion");

        builder.WebHost.ConfigureKestrel(
            k =>
            {
                if (appContext.ServerConfig.WebHttp.IsEnabled)
                {
                    k.Listen(
                        appContext.ServerConfig.WebHttp.ListenAddress.ToIpAddress(),
                        appContext.ServerConfig.WebHttp.ListenPort,
                        options => { options.Protocols = HttpProtocols.Http1; }
                    );
                }
                else
                {
                    k.ListenLocalhost(
                        new WebHttpConfig().ListenPort,
                        options => { options.Protocols = HttpProtocols.Http1; }
                    );
                }

                Environment.SetEnvironmentVariable("ORION_HTTP_PORT", appContext.ServerConfig.WebHttp.ListenPort.ToString());
            }
        );


        Log.Logger = appContext.LoggerConfiguration.CreateLogger();

        builder.Logging.ClearProviders().AddSerilog();

        var app = builder.Build();


        app.Run();
    }
}
