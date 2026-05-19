using System;
using Microsoft.EntityFrameworkCore;
using myfinance.Domain.Entities;
using myfinance.Infrastructure.Context;
using myfinance.Infrastructure.Repositories.Interfaces;

namespace myfinance.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private MyFinanceContext _myfinanceContext;

    public UserRepository(MyFinanceContext myFinanceContext)
    {
        _myfinanceContext = myFinanceContext;
    }

    public async Task<List<User>> GetUsersAsync()
    {
        return await _myfinanceContext.Users.ToListAsync();
    }

    public async Task<User?> FindUserByEmail(string email)
    {
        return await _myfinanceContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task CreateUserAsync(User user)
    {
        await _myfinanceContext.Users.AddAsync(user);

        await _myfinanceContext.SaveChangesAsync();
    }
}
