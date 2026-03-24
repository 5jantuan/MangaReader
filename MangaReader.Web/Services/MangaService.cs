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
                .Include(m => m.Covers)
                .Include(m => m.Chapters)
                    .ThenInclude(c => c.Pages)
                .Include(m => m.Categories)
                .Where(m => m.AuthorId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => new MangaDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    CoverUrl = m.Covers
                        .OrderByDescending(c => c.IsPinned)
                        .Select(c => c.Path)
                        .FirstOrDefault(),
                    TotalViews = m.Chapters.Sum(c => c.Views),
                    Categories = m.Categories
                        .Select(c => c.Name)
                        .ToList(),

                    Chapters = m.Chapters
                        .OrderBy(c => c.Number)
                        .Select(c => new ChapterDto
                        {
                            Id = c.Id,
                            Title = c.Title,
                            Number = c.Number,
                            Views = c.Views,
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

            if (model.SelectedCategoryIds.Any())
            {
                var categories = await _context.Categories
                    .Where(c => model.SelectedCategoryIds.Contains(c.Id))
                    .ToListAsync();

                manga.SetCategories(categories);
            }

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
                .FirstOrDefaultAsync(m => m.Id == model.MangaId);

            if (manga == null)
                throw new InvalidOperationException("Manga not found.");

            var chapter = manga.CreateChapter(model.Title, pagePaths);

            _context.Chapters.Add(chapter);

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
                .Include(m => m.Categories)
                .FirstOrDefaultAsync(m => m.Id == mangaId);
        }

        public async Task<Chapter?> GetChapterForReadingAndIncrementViews(Guid chapterId)
        {
            var chapter = await _context.Chapters
                .Include(c => c.Pages)
                .FirstOrDefaultAsync(c => c.Id == chapterId);

            if (chapter == null)
                return null;

            chapter.IncrementViews();
            await _context.SaveChangesAsync();

            return chapter;
        }

        public async Task<List<CategoryOptionViewModel>> GetCategoriesForSelect()
        {
            return await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new CategoryOptionViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }
    }
}