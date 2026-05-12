using MangaReader.Domain.Entities;
using MangaReader.Domain.Interfaces;
using MangaReader.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MangaReader.Infrastructure.Repositories;

public class BubbleRepository : IBubbleRepository
{
    private readonly AppDbContext _context;

    public BubbleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Bubble bubble)
    {
        await _context.Bubbles.AddAsync(bubble);
    }

    public async Task RemoveByPageIdAsync(Guid pageId)
    {
        var bubbles = await _context.Bubbles
            .Where(b => b.PageId == pageId)
            .ToListAsync();

        _context.Bubbles.RemoveRange(bubbles);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<Bubble>> GetByPageIdAsync(Guid pageId)
    {
        return await _context.Bubbles
            .Include(b => b.Translations)
                .ThenInclude(t => t.Language)
            .Include(b => b.Phrases)
            .Where(b => b.PageId == pageId)
            .OrderBy(b => b.Number)
            .ToListAsync();
    }

    public async Task<Bubble?> GetByIdAsync(Guid id)
    {
        return await _context.Bubbles
            .Include(b => b.Translations)
                .ThenInclude(t => t.Language)
            .Include(b => b.Phrases)
            .FirstOrDefaultAsync(b => b.Id == id);
    }
}