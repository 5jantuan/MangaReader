using System;
using System.Threading.Tasks;
using MangaReader.Domain.Entities;
using MangaReader.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using MangaReader.Infrastructure.Persistence;

namespace MangaReader.Infrastructure.Repositories;

public class LanguageRepository : ILanguageRepository
{
    private readonly AppDbContext _dbContext;

    public LanguageRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Language> GetLanguageByIdAsync(Guid id)
    {
        return await _dbContext.Languages
            .Include(l => l.PhraseTranslations)
            .FirstOrDefaultAsync(l => l.Id == id)
            ?? throw new InvalidOperationException("Language not found");
    }

    public async Task<Language> GetDefaultLanguageAsync()
    {
        // Берем первый язык как дефолтный
        return await _dbContext.Languages
            .Include(l => l.PhraseTranslations)
            .FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("No languages configured");
    }

    public async Task<List<Language>> GetAllAsync()
    {
        return await _dbContext.Languages.ToListAsync();
    }
}