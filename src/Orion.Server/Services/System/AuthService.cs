using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Orion.Core.Server.Data.Config;
using Orion.Core.Server.Data.Config.Sections;
using Orion.Core.Server.Data.Rest;
using Orion.Core.Server.Interfaces.Services.System;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Orion.Server.Services.System;

public class AuthService : IAuthService
{
    private readonly ILogger _logger;
    private readonly OrionServerConfig _orionServerConfig;

    public AuthService(ILogger<AuthService> logger, OrionServerConfig orionServerConfig)
    {
        _logger = logger;
        _orionServerConfig = orionServerConfig;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var response = new LoginResponse()
        {
            IsSuccess = false,
            Message = "Invalid username or password."
        };

        var userEntity = _orionServerConfig.Irc.Opers.Entries.FirstOrDefault(s => s.NickName == request.Username);

        if (userEntity == null)
        {
            _logger.LogWarning("User not found: {Username}", request.Username);
            return response;
        }

        if (!userEntity.IsPasswordValid(request.Password))
        {
            _logger.LogWarning("Invalid password for user: {Username}", request.Username);
            return response;
        }

        var refreshToken = await GenerateRefreshToken(userEntity);
        var jwtToken = GenerateJwtToken(userEntity);
        var tokenExpiration = DateTime.Now.AddMinutes(_orionServerConfig.WebHttp.JwtAuth.ExpirationInMinutes);
        var refreshTokenExpiration = DateTime.Now.AddDays(_orionServerConfig.WebHttp.JwtAuth.RefreshTokenExpiryDays);

        response.IsSuccess = true;

        response.Message = "Login successful.";

        response.AccessToken = jwtToken;
        response.RefreshToken = refreshToken;
        response.AccessTokenExpiresAt = tokenExpiration;
        response.RefreshTokenExpiresAt = refreshTokenExpiration;


        return response;
    }

    private async Task<string> GenerateRefreshToken(OperEntryConfig userEntity)
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string GenerateJwtToken(OperEntryConfig user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.NickName),
            new Claim(JwtRegisteredClaimNames.Jti, user.Id)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_orionServerConfig.WebHttp.JwtAuth.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _orionServerConfig.WebHttp.JwtAuth.Issuer,
            audience: _orionServerConfig.WebHttp.JwtAuth.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_orionServerConfig.WebHttp.JwtAuth.ExpirationInMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
