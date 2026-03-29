using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using myfinance.Application.Services.Interfaces;
using myfinance.Infrastructure.Config;

namespace myfinance.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly MyFinanceSettings _settings;
    public TokenService(IOptions<MyFinanceSettings> settings)
    {
        _settings = settings.Value;
    }
    public async Task<string> GenerateJWT(int userId)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_settings.JwtSecretKey);

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

            var claims = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                ]
            );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = credentials,
                Expires = DateTime.UtcNow.AddHours(2), //Expiração de 2 horas
                Subject = claims
            };

            var token = handler.CreateToken(tokenDescriptor);

            return handler.WriteToken(token);
        }
        catch (Exception ex)
        {
            return "";
        }

    }
}
