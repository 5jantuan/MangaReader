using MangaReader.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class PhrasesController : Controller
{
    private readonly IPhraseService _phraseService;

    public PhrasesController(IPhraseService phraseService)
    {
        _phraseService = phraseService;
    }

    public async Task<IActionResult> Index(Guid pageId)
    {
        var phrases = await _phraseService.GetByPageIdAsync(pageId);
        return View(phrases);
    }
}
