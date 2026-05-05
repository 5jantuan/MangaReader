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

    public async Task<Page?> GetPageWithPhrasesAsync(Guid pageId)
    {
        return await _context.Pages
            .Include(p => p.Phrases)
            .FirstOrDefaultAsync(p => p.Id == pageId);
    }

    public async Task UpdatePageAsync(Page page)
    {
        _context.Pages.Update(page);
        await _context.SaveChangesAsync();
    }
}