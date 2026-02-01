using System;
using System.Threading.Tasks;
using MangaReader.Domain.Entities;

namespace MangaReader.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUserNameAsync(string userName);
    Task<User?> GetByIdAsync(Guid id);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
}