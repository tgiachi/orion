using System.Reflection;
using Orion.Core.Server.Data.Config;
using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Data.Options;
using Orion.Core.Server.Interfaces.Services.System;

namespace Orion.Server.Services.System;

public class VersionService : IVersionService
{
    private readonly AppContextData<OrionServerOptions, OrionServerConfig> _appContextData;
    private readonly ITextTemplateService _templateService;

    public VersionService(AppContextData<OrionServerOptions, OrionServerConfig> appContextData, ITextTemplateService templateService)
    {
        _appContextData = appContextData;
        _templateService = templateService;

        var versionInfo = GetVersionInfo();

        _templateService.AddVariable("version", versionInfo.Version);
        _templateService.AddVariable("codename", versionInfo.CodeName);
        _templateService.AddVariable("commit", versionInfo.GitHash);
        _templateService.AddVariable("branch", versionInfo.Branch);
        _templateService.AddVariable("commit_date", versionInfo.BuildDate);


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
