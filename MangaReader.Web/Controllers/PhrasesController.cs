using MangaReader.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MangaReader.Web.Controllers;

public class PhrasesController : Controller
{
    private readonly IGetPhrasesForPageUseCase _getPhrasesForPage;

    public PhrasesController(IGetPhrasesForPageUseCase getPhrasesForPage)
    {
        _getPhrasesForPage = getPhrasesForPage;
    }

    public async Task<IActionResult> Index(Guid pageId)
    {
        var phrases = await _getPhrasesForPage.ExecuteAsync(pageId);
        return View(phrases);
    }
}
