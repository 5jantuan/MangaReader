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
        return await _context.Phrases
            .Include(p => p.PhraseTranslations)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IReadOnlyList<Phrase>> GetByPageIdAsync(Guid pageId)
    {
        return await _context.Phrases
            .Include(p => p.PhraseTranslations)
            .Where(p => p.PageId == pageId)
            .ToListAsync();
    }

    public async Task AddAsync(Phrase phrase)
    {
        await _context.Phrases.AddAsync(phrase);
    }

    public Task RemoveAsync(Phrase phrase)
    {
        _context.Phrases.Remove(phrase);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Phrase phrase)
    {
        _context.Phrases.Update(phrase);
        return Task.CompletedTask;
    }

    public async Task RemoveByPageIdsAsync(IEnumerable<Guid> pageIds)
    {
        var phrases = await _context.Phrases
            .Where(p => pageIds.Contains(p.PageId))
            .ToListAsync();

        _context.Phrases.RemoveRange(phrases);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}