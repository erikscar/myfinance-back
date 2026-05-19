using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using myfinance.API.Errors;
using myfinance.Application.Services.Interfaces;
using myfinance.Domain.Entities;

namespace myfinance.Infrastructure.Services;

public class TokenService : ITokenService
{
    public async Task<string> GenerateJWT(int userId)
    {
        var handler = new JwtSecurityTokenHandler();
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("u4aQm7M3r0K7mY3rK8v4Yk2kH5q8mD0iT4oY7Qx2N9eV1uL6cP3aR5xF8bW2jH9sQ0zT6nM4cJ1pK7wL2dA=="));

        var claims = new []
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        };

        var token = new JwtSecurityToken(
            signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512),
            claims: claims
        );
        
        return handler.WriteToken(token);
    }
}
