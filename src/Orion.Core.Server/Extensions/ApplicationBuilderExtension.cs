using System.Reflection;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Orion.Core.Server.Data.Directories;
using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Interfaces.Config;
using Orion.Core.Server.Interfaces.Options;
using Orion.Foundations.Extensions;
using Orion.Foundations.Types;
using Orion.Foundations.Utils;
using Serilog;
using Serilog.Formatting.Json;

namespace Orion.Core.Server.Extensions;

public static class ApplicationBuilderExtension
{
    public static AppContextData<TOptions, TConfig> InitApplication<TOptions, TConfig>(
        this IServiceCollection serviceCollection, string appName, params string[]? defaultDirectories
    )
        where TOptions : IOrionServerCmdOptions where TConfig : class, IOrionServerConfig, new()
    {
        var env = Environment.GetEnvironmentVariable(appName.ToSnakeCaseUpper() + "_ENVIRONMENT");
        var appContextData = new AppContextData<TOptions, TConfig>
        {
            AppName = appName,
            Environment = env ?? "Production"
        };

        serviceCollection.AddSingleton(new AppNameData(appContextData.AppName));

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
        defaultDirectories ??= new List<string>().ToArray();

        var directoriesConfig = new DirectoriesConfig(parsedOptions.Value.RootDirectory, defaultDirectories);

        if (parsedOptions.Value.ConfigFile == null)
        {
            parsedOptions.Value.ConfigFile = appName.ToSnakeCase() + ".yml";
        }

        serviceCollection.AddSingleton(directoriesConfig);
        appContextData.Config =
            directoriesConfig.LoadConfig<TConfig>(serviceCollection, parsedOptions.Value.ConfigFile);


        serviceCollection.AddSingleton<IOrionServerConfig>(appContextData.Config);
        serviceCollection.AddSingleton<TConfig>(appContextData.Config);

        appContextData.Directories = directoriesConfig;

        appContextData.Options = parsedOptions.Value;

        if (Environment.GetEnvironmentVariable(appName.ToSnakeCaseUpper() + "_DEBUG") != null)
        {
            appContextData.Options.IsDebug = true;
        }


        appContextData.LoggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Is(parsedOptions.Value.LogLevel.ToSerilogLogLevel())
            .WriteTo.File(
                new JsonFormatter(),
                rollingInterval: RollingInterval.Day,
                path: Path.Combine(directoriesConfig["Logs"], $"{appName}_.log")
            );

        if (parsedOptions.Value.IsDebug)
        {
            appContextData.LoggerConfiguration
                .MinimumLevel.Is(LogLevelType.Debug.ToSerilogLogLevel());
        }


        if (appContextData.Config.Debug.SaveRawPackets)
        {
            appContextData.LoggerConfiguration.WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(e => e.Properties.ContainsKey("SourceContext") &&
                                             e.Properties["SourceContext"].ToString().Contains("NetworkTransportManager")
                )
                .MinimumLevel.Debug()
                .WriteTo.File(
                    Path.Combine(directoriesConfig["Logs"], "packets", "raw_.log"),
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
