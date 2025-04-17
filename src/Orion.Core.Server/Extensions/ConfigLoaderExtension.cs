using Microsoft.Extensions.DependencyInjection;
using Orion.Core.Server.Data.Directories;
using Orion.Core.Server.Interfaces.Config;
using Orion.Foundations.Extensions;

namespace Orion.Core.Server.Extensions;

public static class ConfigLoaderExtension
{
    public static TConfig LoadConfig<TConfig>(
        this DirectoriesConfig directoriesConfig, IServiceCollection serviceCollection, string configFileName = "orion.yml"
    ) where TConfig : IOrionServerConfig, new()
    {
        var configFilePath = Path.Combine(directoriesConfig.Root, configFileName);

        if (!File.Exists(configFilePath))
        {
            var newConfig = new TConfig();

            SaveConfig(newConfig, configFilePath);
        }

        var config = LoadConfig(new TConfig(), configFilePath);


        SaveConfig(config, configFilePath);

        return config;
    }


    private static void SaveConfig<TConfig>(this TConfig config, string configFilePath) where TConfig : IOrionServerConfig
    {
        var configContent = config.ToYaml();
        File.WriteAllText(configFilePath, configContent);
    }

    private static TConfig LoadConfig<TConfig>(this TConfig config, string configFilePath) where TConfig : IOrionServerConfig
    {
        var configContent = File.ReadAllText(configFilePath);
        var cfg = configContent.FromYaml<TConfig>();


        return cfg;
    }
}
