using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MangaReader.Domain.Entities;

namespace MangaReader.Domain.Interfaces;

public interface ILanguageRepository
{
    Task<Language> GetLanguageByIdAsync(Guid id);
    Task<Language> GetDefaultLanguageAsync();
    Task<List<Language>> GetAllAsync();
}