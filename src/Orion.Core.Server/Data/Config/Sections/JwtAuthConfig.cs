using System.Security.Cryptography;
using Orion.Foundations.Extensions;

namespace Orion.Core.Server.Data.Config.Sections;

public class JwtAuthConfig
{
    public string Issuer { get; set; } = "Orion";
    public string Audience { get; set; } = "Orion";
    public string Secret { get; set; } = RandomNumberGenerator.GetBytes(128).ToBase64();

    public int ExpirationInMinutes { get; set; } = 60 * 24 * 31; // 31 day

    public int RefreshTokenExpiryDays { get; set; } = 1;
}
