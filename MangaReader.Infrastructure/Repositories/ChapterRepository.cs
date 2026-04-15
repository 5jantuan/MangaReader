using MangaReader.Domain.Entities;
using MangaReader.Domain.Interfaces;
using MangaReader.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MangaReader.Infrastructure.Repositories;

public class ChapterRepository : IChapterRepository
{
    private readonly AppDbContext _context;

    public ChapterRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Chapter?> GetByIdWithPagesAsync(Guid chapterId)
    {
        return await _context.Chapters
            .Include(c => c.Pages)
            .FirstOrDefaultAsync(c => c.Id == chapterId);
    }

    public async Task UpdateAsync(Chapter chapter)
    {
        _context.Chapters.Update(chapter);
        await _context.SaveChangesAsync();
    }
}