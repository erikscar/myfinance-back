using System.Security.Claims;
using myfinance.Application.Services.Interfaces;
using myfinance.Domain.DTOS;
using myfinance.Domain.Entities;
using myfinance.Infrastructure.Repositories.Interfaces;

namespace myfinance.Application.Services;

public class UserService(
    IUserRepository userRepository, 
    IPasswordService passwordService,
    ITokenService tokenService
    ) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordService _passwordService = passwordService; 
    private readonly ITokenService _tokenService = tokenService;

    public async Task<List<User>> GetUsersAsync()
    {
        var users = await _userRepository.GetUsersAsync();
        
        return users;
    }

    public async Task<string> LoginUserAsync(LoginRequestDTO userData)
    {
        User user = await _userRepository.FindUserByEmail(userData.Email);

        if (user is null)
        {
            throw new Exception("User does not exists");
        }

        bool isPasswordCorrect = _passwordService.Verify(userData.Password, user.PasswordHash);

        if (!isPasswordCorrect) {
            throw new Exception("Password is incorrect");
        }

        return await _tokenService.GenerateJWT(user.Id);
    }

    public async Task RegisterUserAsync(RegisterRequestDTO userData)
    {
        var password = _passwordService.Hash(userData.Password);
        
        User user = new User(
            userData.Name,
            userData.Email,
            password
        );
        
        await _userRepository.CreateUserAsync(user);
    }

}
