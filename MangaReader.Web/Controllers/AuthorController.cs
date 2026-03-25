using System.Security.Claims;
using MangaReader.Web.Services;
using MangaReader.Web.ViewModels.Chapter;
using MangaReader.Web.ViewModels.Manga;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangaReader.Domain.Interfaces;

namespace MangaReader.Web.Controllers
{
    [Authorize]
    public class AuthorController : Controller
    {
        private readonly MangaService _mangaService;
        private readonly FileService _fileService;
        private readonly IUserRepository _userRepository;


        public AuthorController(
            MangaService mangaService,
            FileService fileService,
            IUserRepository userRepository)
        {
            _mangaService = mangaService;
            _fileService = fileService;
            _userRepository = userRepository;
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
        public IActionResult CreateChapter(Guid mangaId)
        {
            return View(new CreateChapterViewModel { MangaId = mangaId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateChapter(CreateChapterViewModel model, List<IFormFile> pages)
        {
            var imageUrls = new List<string>();

            foreach (var file in pages)
            {
                var url = await _fileService.SaveFile(file);
                imageUrls.Add(url);
            }

            await _mangaService.CreateChapter(model, imageUrls);

            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> ReadChapter(Guid chapterId)
        {
            var chapter = await _mangaService.GetChapterForReadingAndIncrementViews(chapterId);

            if (chapter == null)
                return NotFound();

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
    }
}