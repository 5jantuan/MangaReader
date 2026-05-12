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
}