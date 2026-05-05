using System.Security.Claims;
using System.Globalization;
using MangaReader.Web.Services;
using MangaReader.Web.ViewModels.Chapter;
using MangaReader.Web.ViewModels.Manga;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangaReader.Domain.Interfaces;
using MangaReader.Domain.Enums;
using MangaReader.Domain.Entities;
using MangaReader.Application.Interfaces;

namespace MangaReader.Web.Controllers
{
    [Authorize]
    public class AuthorController : Controller
    {
        private readonly MangaService _mangaService;
        private readonly FileService _fileService;
        private readonly IUserRepository _userRepository;
        private readonly DemoTranslationSeeder _demoTranslationSeeder;
        private readonly IChapterProcessingService _chapterProcessingService;
        private readonly IChapterProcessingQueue _chapterProcessingQueue;
        private readonly IChapterRepository _chapterRepository;
        private readonly IPhraseRepository _phraseRepository;

        public AuthorController(
            MangaService mangaService,
            FileService fileService,
            IUserRepository userRepository,
            DemoTranslationSeeder demoTranslationSeeder,
            IChapterProcessingService chapterProcessingService,
            IChapterProcessingQueue chapterProcessingQueue,
            IChapterRepository chapterRepository,
            IPhraseRepository phraseRepository)
        {
            _mangaService = mangaService;
            _fileService = fileService;
            _userRepository = userRepository;
            _demoTranslationSeeder = demoTranslationSeeder;
            _chapterProcessingService = chapterProcessingService;
            _chapterProcessingQueue = chapterProcessingQueue;
            _chapterRepository = chapterRepository;
            _phraseRepository = phraseRepository;
        }

        private Guid GetCurrentUserId()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdValue))
                throw new UnauthorizedAccessException("User is not authenticated.");

            return Guid.Parse(userIdValue);
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var userId = GetCurrentUserId();

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return NotFound();

            var mangas = await _mangaService.GetMyMangas(userId);

            var model = new MangaReader.Web.ViewModels.Author.AuthorDashboardViewModel
            {
                UserName = user.UserName,
                AvatarPath = user.AvatarPath,
                ProjectsCount = mangas.Count,
                ChaptersCount = mangas.Sum(m => m.Chapters.Count),
                TotalViews = mangas.Sum(m => m.TotalViews),
                Mangas = mangas
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CreateManga()
        {
            var model = new CreateMangaViewModel
            {
                AvailableCategories = await _mangaService.GetCategoriesForSelect()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateManga(CreateMangaViewModel model, IFormFile? cover)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableCategories = await _mangaService.GetCategoriesForSelect();
                return View(model);
            }

            var userId = GetCurrentUserId();

            string? coverUrl = null;
            if (cover != null)
                coverUrl = await _fileService.SaveFile(cover);

            await _mangaService.CreateManga(model, coverUrl, userId);

            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> CreateChapter(Guid mangaId)
        {
            var manga = await _mangaService.GetMangaById(mangaId);

            if (manga == null)
                return NotFound();

            var nextNumber = manga.Chapters.Any()
                ? manga.Chapters.Max(c => c.Number) + 1
                : 1;

            var model = new CreateChapterViewModel
            {
                MangaId = mangaId,
                Number = nextNumber
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateChapter(CreateChapterViewModel model, List<IFormFile> pages)
        {
            if (!ModelState.IsValid)
                return View(model);

            var manga = await _mangaService.GetMangaById(model.MangaId);

            if (manga == null)
                return NotFound();

            model.Number = manga.Chapters.Any()
                ? manga.Chapters.Max(c => c.Number) + 1
                : 1;

            var imageUrls = new List<string>();

            foreach (var file in pages)
            {
                var url = await _fileService.SaveFile(file);
                imageUrls.Add(url);
            }

            var chapterId = await _mangaService.CreateChapter(model, imageUrls);

            return RedirectToAction("ReviewChapterPages", new { chapterId });
        }

        [HttpGet]
        public async Task<IActionResult> ReviewChapterPages(Guid chapterId)
        {
            var chapter = await _chapterRepository.GetByIdWithPagesAsync(chapterId);

            if (chapter == null)
                return NotFound();

            var model = new ReviewChapterPagesViewModel
            {
                ChapterId = chapter.Id,
                ChapterTitle = chapter.Title,
                ChapterNumber = chapter.Number,
                Pages = chapter.Pages
                    .OrderBy(p => p.Number)
                    .Select(p => new ReviewPageViewModel
                    {
                        PageId = p.Id,
                        Number = p.Number,
                        ImagePath = p.ImagePath
                    })
                    .ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmChapterPages(Guid chapterId)
        {
            await _chapterProcessingService.ProcessChapterAsync(chapterId);

            return RedirectToAction("ChapterStatus", new { chapterId });
        }

        public async Task<IActionResult> ChapterStatus(Guid chapterId)
        {
            var chapter = await _chapterRepository.GetByIdWithPagesAsync(chapterId);

            if (chapter == null)
                return NotFound();

            var model = new ChapterProcessingStatusViewModel
            {
                ChapterId = chapter.Id,
                ChapterTitle = chapter.Title,
                ChapterStatus = chapter.ProcessingStatus,
                TotalPages = chapter.Pages.Count,
                CompletedPages = chapter.Pages.Count(p => p.ProcessingStatus == PageProcessingStatus.Completed),
                Pages = chapter.Pages
                    .OrderBy(p => p.Number)
                    .Select(p => new PageProcessingStatusViewModel
                    {
                        PageId = p.Id,
                        Number = p.Number,
                        ImagePath = p.ImagePath,
                        Status = p.ProcessingStatus
                    })
                    .ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ReviewOcrPage(Guid pageId)
        {
            var page = await _chapterRepository.GetPageWithPhrasesAsync(pageId);

            if (page == null)
                return NotFound();

            var model = new ReviewOcrPageViewModel
            {
                ChapterId = page.ChapterId,
                PageId = page.Id,
                PageNumber = page.Number,
                ImagePath = page.ImagePath,
                Status = page.ProcessingStatus,
                Phrases = page.Phrases
                    .OrderBy(p => p.Y)
                    .ThenBy(p => p.X)
                    .Select(p => new OcrPhraseReviewViewModel
                    {
                        PhraseId = p.Id,
                        Text = p.Text,
                        X = p.X,
                        Y = p.Y,
                        Width = p.Width,
                        Height = p.Height,
                        Confidence = p.Confidence
                    })
                    .ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ReadChapter(Guid chapterId)
        {
            var chapter = await _mangaService.GetChapterForReadingAndIncrementViews(chapterId);

            if (chapter == null)
                return NotFound();

            var userId = GetCurrentUserId();
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return NotFound();

            ViewBag.PreferredLanguageId = user.PreferredLanguageId;

            return View(chapter);
        }

        [HttpGet]
        public async Task<IActionResult> MangaDetails(Guid mangaId)
        {
            var manga = await _mangaService.GetMangaById(mangaId);

            if (manga == null)
                return NotFound();

            return View(manga);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = GetCurrentUserId();

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return NotFound();

            var mangas = await _mangaService.GetMyMangas(userId);
            var totalViews = mangas.Sum(m => m.TotalViews);

            var model = new MangaReader.Web.ViewModels.Author.AuthorProfileViewModel
            {
                UserName = user.UserName,
                AvatarPath = user.AvatarPath,
                About = user.About,
                TelegramUrl = user.TelegramUrl,
                InstagramUrl = user.InstagramUrl,
                TikTokUrl = user.TikTokUrl,
                ProjectsCount = mangas.Count
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var userId = GetCurrentUserId();

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return NotFound();

            var model = new MangaReader.Web.ViewModels.Author.EditAuthorProfileViewModel
            {
                About = user.About,
                TelegramUrl = user.TelegramUrl,
                InstagramUrl = user.InstagramUrl,
                TikTokUrl = user.TikTokUrl
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(
            MangaReader.Web.ViewModels.Author.EditAuthorProfileViewModel model,
            IFormFile? avatar)
        {
            var userId = GetCurrentUserId();

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return NotFound();

            string? avatarPath = null;
            if (avatar != null)
                avatarPath = await _fileService.SaveFile(avatar);

            user.UpdateProfile(
                model.About,
                model.TelegramUrl,
                model.InstagramUrl,
                model.TikTokUrl,
                avatarPath);

            await _userRepository.UpdateAsync(user);

            return RedirectToAction("Profile");
        }

        [HttpPost]
        public async Task<IActionResult> SeedDemoTranslation(Guid chapterId, Guid mangaId)
        {
            var russianLanguageId = Guid.Parse("22222222-2222-2222-2222-222222222222");

            await _demoTranslationSeeder.SeedFrierenDemoAsync(chapterId, russianLanguageId);

            return RedirectToAction("MangaDetails", new { mangaId });
        }

        [HttpPost]
        public async Task<IActionResult> AddManualOcrPhrase(
            Guid pageId,
            string text,
            string x,
            string y,
            string width,
            string height)
        {
            var parsedX = decimal.Parse(x, CultureInfo.InvariantCulture);
            var parsedY = decimal.Parse(y, CultureInfo.InvariantCulture);
            var parsedWidth = decimal.Parse(width, CultureInfo.InvariantCulture);
            var parsedHeight = decimal.Parse(height, CultureInfo.InvariantCulture);

            var phrase = new Phrase(
                pageId,
                text,
                parsedX,
                parsedY,
                parsedWidth,
                parsedHeight,
                1m
            );

            await _phraseRepository.AddAsync(phrase);
            await _phraseRepository.SaveChangesAsync();

            return RedirectToAction("ReviewOcrPage", new { pageId });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOcrPhrase(
            Guid phraseId,
            Guid pageId,
            string text)
        {
            var phrase = await _phraseRepository.GetByIdAsync(phraseId);

            if (phrase == null)
                return NotFound();

            phrase.UpdateText(text);

            await _phraseRepository.UpdateAsync(phrase);
            await _phraseRepository.SaveChangesAsync();

            return RedirectToAction("ReviewOcrPage", new { pageId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOcrPhrase(Guid phraseId, Guid pageId)
        {
            var phrase = await _phraseRepository.GetByIdAsync(phraseId);

            if (phrase == null)
                return NotFound();

            await _phraseRepository.RemoveAsync(phrase);
            await _phraseRepository.SaveChangesAsync();

            return RedirectToAction("ReviewOcrPage", new { pageId });
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmOcrPage(Guid pageId)
        {
            var page = await _chapterRepository.GetPageWithPhrasesAsync(pageId);

            if (page == null)
                return NotFound();

            page.MarkOcrCompleted();

            await _chapterRepository.UpdatePageAsync(page);

            return RedirectToAction("ChapterStatus", new { chapterId = page.ChapterId });
        }

        [HttpPost]
        public async Task<IActionResult> StartPageTranslation(Guid pageId)
        {
            var page = await _chapterRepository.GetPageWithPhrasesAsync(pageId);

            if (page == null)
                return NotFound();

            page.MarkTranslationProcessing();

            await _chapterRepository.UpdatePageAsync(page);

            // Временная имитация: позже тут будет реальный сервис перевода
            page.MarkCompleted();

            await _chapterRepository.UpdatePageAsync(page);

            return RedirectToAction("ChapterStatus", new { chapterId = page.ChapterId });
        }
    }
}