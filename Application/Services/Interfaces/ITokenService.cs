namespace myfinance.Application.Services.Interfaces;

public interface ITokenService
{
    Task<string> GenerateJWT(int userId);
}
