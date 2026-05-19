using System;
using myfinance.Domain.DTOS;
using myfinance.Domain.Entities;

namespace myfinance.Application.Services.Interfaces;

public interface IUserService
{
    Task<List<User>> GetUsersAsync();
    Task<string> LoginUserAsync(LoginRequestDTO userData);
    Task RegisterUserAsync(RegisterRequestDTO userData);
}
