using MangaReader.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace MangaReader.Web.Controllers
{
    public class MangaController : Controller
    {
        private readonly MangaService _mangaService;

        public MangaController(MangaService mangaService)
        {
            _mangaService = mangaService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid? categoryId)
        {
            var model = await _mangaService.GetCatalog(categoryId);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid mangaId)
        {
            var manga = await _mangaService.GetPublicMangaById(mangaId);

            if (manga == null)
                return NotFound();

            return View(manga);
        }
    }
}