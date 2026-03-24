using System;
using System.Threading.Tasks;
using MangaReader.Domain.Entities;
using MangaReader.Domain.Interfaces;
using MangaReader.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MangaReader.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUserNameAsync(string userName)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.UserName == userName);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        // EF уже отслеживает сущность, но на всякий случай:
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    // проверка существования пользователя
    public async Task<bool> ExistsByUserNameAsync(string userName)
    {
        return await _context.Users
            .AnyAsync(u => u.UserName == userName);
    }
}