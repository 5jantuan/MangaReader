using MangaReader.Domain.Entities;
using MangaReader.Infrastructure.Persistence;
using MangaReader.Web.Models.Manga;
using MangaReader.Web.ViewModels.Chapter;
using MangaReader.Web.ViewModels.Manga;
using Microsoft.EntityFrameworkCore;

namespace MangaReader.Web.Services
{
    public class MangaService
    {
        private readonly AppDbContext _context;

        public MangaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MangaDto>> GetMyMangas(Guid userId)
        {
            return await _context.Mangas
                .AsNoTracking()
                .Include(m => m.Chapters)
                    .ThenInclude(c => c.Pages)
                .Where(m => m.AuthorId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => new MangaDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Chapters = m.Chapters
                        .OrderBy(c => c.Number)
                        .Select(c => new ChapterDto
                        {
                            Id = c.Id,
                            Title = c.Title,
                            Number = c.Number,
                            Pages = c.Pages
                                .OrderBy(p => p.Number)
                                .Select(p => p.ImagePath)
                                .ToList()
                        })
                        .ToList()
                })
                .ToListAsync();
        }

        public async Task CreateManga(CreateMangaViewModel model, string? coverUrl, Guid userId)
        {
            var manga = Manga.Create(model.Title, model.Description, userId);

            _context.Mangas.Add(manga);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(coverUrl))
            {
                var cover = new MangaCover(manga.Id, coverUrl);
                cover.Pin();

                _context.MangaCovers.Add(cover);
                await _context.SaveChangesAsync();
            }
        }

        public async Task CreateChapter(CreateChapterViewModel model, List<string> pagePaths)
        {
            var manga = await _context.Mangas
                .Include(m => m.Chapters)
                    .ThenInclude(c => c.Pages)
                .FirstOrDefaultAsync(m => m.Id == model.MangaId);

            if (manga == null)
                throw new InvalidOperationException("Manga not found.");

            manga.CreateChapter(model.Title, pagePaths);

            await _context.SaveChangesAsync();
        }

        public async Task<Chapter?> GetChapterForReading(Guid chapterId)
        {
            return await _context.Chapters
                .AsNoTracking()
                .Include(c => c.Pages)
                .FirstOrDefaultAsync(c => c.Id == chapterId);
        }

        public async Task<Manga?> GetMangaById(Guid mangaId)
        {
            return await _context.Mangas
                .AsNoTracking()
                .Include(m => m.Chapters)
                    .ThenInclude(c => c.Pages)
                .Include(m => m.Covers)
                .FirstOrDefaultAsync(m => m.Id == mangaId);
        }
    }
}