namespace Orion.Core.Irc.Server.Data.Rest;

public class LoginResponse
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? AccessTokenExpiresAt { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }
}
