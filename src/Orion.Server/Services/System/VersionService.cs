using System.Reflection;
using Orion.Core.Server.Data.Config;
using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Data.Options;
using Orion.Core.Server.Interfaces.Services.System;

namespace Orion.Server.Services.System;

public class VersionService : IVersionService
{
    private readonly AppContextData<OrionServerOptions, OrionServerConfig> _appContextData;

    public VersionService(AppContextData<OrionServerOptions, OrionServerConfig> appContextData)
    {
        _appContextData = appContextData;
    }

    public VersionInfoData GetVersionInfo()
    {
        var version = typeof(VersionService).Assembly.GetName().Version;

        var codename = Assembly.GetExecutingAssembly()
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(attr => attr.Key == "Codename")
            ?.Value;

        return new VersionInfoData(
            _appContextData.AppName,
            codename,
            version.ToString(),
            ThisAssembly.Git.Commit,
            ThisAssembly.Git.Branch,
            ThisAssembly.Git.CommitDate
        );
    }
}
