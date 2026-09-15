using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OrderManagement.Application.Abstractions;

namespace OrderManagement.Infrastructure.Security;

public sealed class JwtTokenService(IConfiguration configuration) : IAuthTokenService
{
    public string CreateToken(string email)
    {
        var issuer = GetRequiredSetting("Jwt:Issuer");
        var audience = GetRequiredSetting("Jwt:Audience");
        var key = GetRequiredSetting("Jwt:Key");

        var token = new JwtSecurityToken(
            issuer,
            audience,
            [new Claim(ClaimTypes.Email, email)],
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GetRequiredSetting(string key) =>
        configuration[key] ?? throw new InvalidOperationException($"Missing required configuration value '{key}'.");
}
