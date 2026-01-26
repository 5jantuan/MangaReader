using MangaReader.Domain.Entities;
using MangaReader.Domain.Interfaces;
using MangaReader.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MangaReader.Infrastructure.Repositories;

public class PhraseRepository : IPhraseRepository
{
    private readonly AppDbContext _context;

    public PhraseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Phrase?> GetByIdAsync(Guid id)
    {
        return await _context.Phrases.FindAsync(id);
    }

    public async Task<IReadOnlyList<Phrase>> GetByPageIdAsync(Guid pageId)
    {
        return await _context.Phrases
            .Where(p => p.PageId == pageId)
            .ToListAsync();
    }

    public async Task AddAsync(Phrase phrase)
    {
        await _context.Phrases.AddAsync(phrase);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(Phrase phrase)
    {
        _context.Phrases.Remove(phrase);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Phrase phrase)
    {
        _context.Phrases.Update(phrase); // помечаем сущность как изменённую
        await _context.SaveChangesAsync(); // сохраняем изменения в БД
    }

}