using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Orion.Core.Server.Data.Config.Sections;
using Orion.Foundations.Extensions;


namespace Orion.Core.Server.Web.Extensions;

public static class KestrelConfigExtension
{
    public static IWebHostBuilder ConfigureKestrelFromConfig(
        this ConfigureWebHostBuilder config, WebHttpConfig serverConfig
    )
    {
        return config.ConfigureKestrel(
            k =>
            {
                if (serverConfig.IsEnabled)
                {
                    k.Listen(
                        serverConfig.ListenAddress.ToIpAddress(),
                        serverConfig.ListenPort,
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

                Environment.SetEnvironmentVariable("ORION_HTTP_PORT", serverConfig.ListenPort.ToString());
            }
        );
    }
}
