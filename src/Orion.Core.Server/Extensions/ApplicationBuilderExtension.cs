using System.Reflection;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Orion.Core.Extensions;
using Orion.Core.Server.Data.Config;
using Orion.Core.Server.Data.Directories;
using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Interfaces.Config;
using Orion.Core.Server.Interfaces.Options;
using Orion.Core.Server.Types;
using Orion.Core.Types;
using Orion.Core.Utils;
using Serilog;
using Serilog.Formatting.Json;


namespace Orion.Core.Server.Extensions;

public static class ApplicationBuilderExtension
{
    public static AppContextData<TOptions, TConfig> InitApplication<TOptions, TConfig>(
        this IServiceCollection serviceCollection, string appName
    )
        where TOptions : IOrionServerCmdOptions where TConfig : OrionServerConfig, new()
    {
        var env = Environment.GetEnvironmentVariable(appName.ToSnakeCaseUpper() + "_ENVIRONMENT");
        var appContextData = new AppContextData<TOptions, TConfig>()
        {
            AppName = appName,
            Environment = env ?? "Production"
        };

        var parsedOptions = Parser.Default.ParseArguments<TOptions>(Environment.GetCommandLineArgs());

        if (parsedOptions is null)
        {
            Console.WriteLine("Error parsing command line arguments.");
            Environment.Exit(1);
        }

        if (parsedOptions.Errors.Any())
        {
            foreach (var error in parsedOptions.Errors)
            {
                Console.WriteLine(error.ToString());
            }

            Console.WriteLine("Error parsing command line arguments.");
            Environment.Exit(1);
        }

        var rootDirectoryFromEnv = Environment.GetEnvironmentVariable(appName.ToSnakeCaseUpper() + "_SERVER_ROOT");

        if (rootDirectoryFromEnv != null)
        {
            parsedOptions.Value.RootDirectory = rootDirectoryFromEnv;
        }

        parsedOptions.Value.RootDirectory ??= Path.Combine(Directory.GetCurrentDirectory(), appName.ToSnakeCase() + "_root");

        serviceCollection.AddSingleton(parsedOptions);

        var directoriesConfig = new DirectoriesConfig(parsedOptions.Value.RootDirectory);

        serviceCollection.AddSingleton(directoriesConfig);

        appContextData.ServerConfig =
            directoriesConfig.LoadConfig<TConfig>(serviceCollection, parsedOptions.Value.ConfigFile);

        serviceCollection.AddSingleton<TConfig>(appContextData.ServerConfig);

        appContextData.DirectoriesConfig = directoriesConfig;

        appContextData.ServerOptions = parsedOptions.Value;

        if (Environment.GetEnvironmentVariable(appName.ToSnakeCaseUpper() + "_DEBUG") != null)
        {
            appContextData.ServerOptions.IsDebug = true;
        }


        appContextData.LoggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Is(parsedOptions.Value.LogLevel.ToSerilogLogLevel())
            .WriteTo.Console()
            .WriteTo.File(
                formatter: new JsonFormatter(),
                rollingInterval: RollingInterval.Day,
                path: Path.Combine(directoriesConfig[DirectoryType.Logs], $"{appName}_.log")
            );

        if (parsedOptions.Value.IsDebug)
        {
            appContextData.LoggerConfiguration
                .MinimumLevel.Is(LogLevelType.Debug.ToSerilogLogLevel());
        }


        if (appContextData.ServerConfig.Debug.SaveRawPackets)
        {
            appContextData.LoggerConfiguration.WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(e => e.Properties.ContainsKey("SourceContext") &&
                                             e.Properties["SourceContext"].ToString().Contains("NetworkTransportManager")
                )
                .MinimumLevel.Debug()
                .WriteTo.File(
                    Path.Combine(directoriesConfig[DirectoryType.Logs], "packets", "raw_.log"),
                    rollingInterval: RollingInterval.Day
                )
            );
        }


        return appContextData;
    }

    public static void ShowHeader<TOption>(this TOption options, Assembly assembly, string headerFile = "Assets.header.txt")
        where TOption : IOrionServerCmdOptions
    {
        if (options.ShowHeader)
        {
            var header = ResourceUtils.ReadEmbeddedResource(headerFile, assembly);

            foreach (var line in header.Split(Environment.NewLine))
            {
                Console.WriteLine(line);
            }
        }
    }
}
