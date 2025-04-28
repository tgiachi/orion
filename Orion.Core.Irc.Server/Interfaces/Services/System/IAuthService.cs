using Orion.Core.Irc.Server.Data.Rest;
using Orion.Core.Server.Interfaces.Services.Base;

namespace Orion.Core.Irc.Server.Interfaces.Services.System;

public interface IAuthService : IOrionService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}
