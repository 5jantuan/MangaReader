using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangaReader.Web.Services;
using MangaReader.Web.ViewModels.Chapter;
using MangaReader.Web.ViewModels.Manga;

[Authorize]
public class AuthorController : Controller
{
    private readonly MangaService _mangaService;
    private readonly FileService _fileService;

    public AuthorController(MangaService mangaService, FileService fileService)
    {
        _mangaService = mangaService;
        _fileService = fileService;
    }

    private Guid GetCurrentUserId()
    {
        return Guid.Parse("11111111-1111-1111-1111-111111111111");
    }

    public async Task<IActionResult> Dashboard()
    {
        var userId = GetCurrentUserId();
        var mangas = await _mangaService.GetMyMangas(userId);

        return View(mangas);
    }

    public IActionResult CreateManga()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateManga(CreateMangaViewModel model, IFormFile? cover)
    {
        var userId = GetCurrentUserId();

        string? coverUrl = null;
        if (cover != null)
            coverUrl = await _fileService.SaveFile(cover);

        await _mangaService.CreateManga(model, coverUrl, userId);

        return RedirectToAction("Dashboard");
    }

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
}