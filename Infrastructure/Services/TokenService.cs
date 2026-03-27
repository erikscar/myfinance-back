using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using myfinance.Application.Services.Interfaces;

namespace myfinance.Infrastructure.Services;

public class TokenService : ITokenService
{
    public async Task<string> GenerateJWT()
    {
        var handler = new JwtSecurityTokenHandler();
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("CHAVEJWT"));

        var claims = new []
        {
            new Claim(ClaimTypes.NameIdentifier, "Erik Scarcela Araujo")
        };

        var token = new JwtSecurityToken(
            signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512),
            claims: claims
        );
        
        return handler.WriteToken(token);
    }
}
