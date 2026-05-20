using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using myfinance.API.Errors;
using myfinance.Application.Services.Interfaces;
using myfinance.Domain.Entities;
using Microsoft.Extensions.Options;
using myfinance.Infrastructure.Config;

namespace myfinance.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IOptions<MyFinanceSettings> _settings;
    public TokenService(IOptions<MyFinanceSettings> settings)
    {
        _settings = settings;
    }
    public async Task<string> GenerateJWT(int userId)
    {
        var handler = new JwtSecurityTokenHandler();
        
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Value.JwtSecretKey));

        var claims = new []
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        };

        var token = new JwtSecurityToken(
            signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512),
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2)
        );
        
        return handler.WriteToken(token);
    }
}
