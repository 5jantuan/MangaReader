using MangaReader.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MangaReader.Web.Controllers;

public class PhrasesController : Controller
{
    private readonly IPhraseRepository _phraseRepository;

    public PhrasesController(IPhraseRepository phraseRepository)
    {
        _phraseRepository = phraseRepository;
    }

    public async Task<IActionResult> Index(Guid pageId)
    {
        var phrases = await _phraseRepository.GetByPageIdAsync(pageId);
        return View(phrases);
    }
}
