using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MangaReader.Application.UseCases;
using MangaReader.Web.ViewModels.Manga;
using MangaReader.Web.ViewModels.Chapter;

public class AuthorController : Controller
{
    private readonly MangaService _mangaService;
    private readonly FileService _fileService;

    public AuthorController(MangaService mangaService, FileService fileService)
    {
        _mangaService = mangaService;
        _fileService = fileService;
    }

    public async Task<IActionResult> Dashboard()
    {
        var mangas = _mangaService.GetMyMangas();
        return View(mangas);
    }

    public IActionResult CreateManga()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateManga(CreateMangaViewModel model, IFormFile cover)
    {
        if (!ModelState.IsValid)
            return View(model);

        string? coverUrl = null;

        if (cover != null)
            coverUrl = await _fileService.SaveFile(cover);

        await _mangaService.CreateManga(model, coverUrl);

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