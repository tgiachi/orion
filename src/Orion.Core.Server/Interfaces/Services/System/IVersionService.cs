using Orion.Core.Server.Data.Internal;

namespace Orion.Core.Server.Interfaces.Services.System;

public interface IVersionService
{
    VersionInfoData GetVersionInfo();
}
