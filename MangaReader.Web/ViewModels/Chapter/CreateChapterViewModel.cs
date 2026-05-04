using Microsoft.AspNetCore.Http;

namespace MangaReader.Web.ViewModels.Chapter;

public class CreateChapterViewModel
{
    public Guid MangaId { get; set; }

    public string Title { get; set; } = string.Empty;

    public int Number { get; set; }

    public List<IFormFile> Pages { get; set; } = new();
}