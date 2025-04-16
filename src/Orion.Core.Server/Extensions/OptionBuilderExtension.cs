using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Orion.Core.Extensions;
using Orion.Core.Server.Data.Directories;
using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Interfaces.Options;
using Orion.Core.Server.Types;
using Orion.Core.Types;
using Serilog;
using Serilog.Formatting.Json;


namespace Orion.Core.Server.Extensions;

public static class OptionBuilderExtension
{
    public static AppContextData InitApplication<TOptions>(this IServiceCollection serviceCollection, string appName)
        where TOptions : IOrionServerCmdOptions
    {
        var env = Environment.GetEnvironmentVariable(appName.ToSnakeCaseUpper() + "_ENVIRONMENT");
        var appContextData = new AppContextData()
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

        parsedOptions.Value.RootDirectory ??= Path.Combine(Directory.GetCurrentDirectory(), appName.ToSnakeCaseUpper());

        serviceCollection.AddSingleton(parsedOptions);

        var directoriesConfig = new DirectoriesConfig(parsedOptions.Value.RootDirectory);

        appContextData.DirectoriesConfig = directoriesConfig;

        appContextData.ServerOptions = parsedOptions.Value;


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


        return appContextData;
    }
}
