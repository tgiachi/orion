using Orion.Core.Server.Data.Internal;
using Orion.Core.Server.Interfaces.Services.Base;

namespace Orion.Core.Server.Interfaces.Services.System;

public interface IVersionService : IOrionService
{
    VersionInfoData GetVersionInfo();
}
