using MangaReader.Web.Models.Manga;

namespace MangaReader.Web.ViewModels.Author;

public class AuthorDashboardViewModel
{
    public string UserName { get; set; } = string.Empty;
    public string? AvatarPath { get; set; }

    public int ProjectsCount { get; set; }
    public int ChaptersCount { get; set; }
    public int TotalViews { get; set; }

    public List<MangaDto> Mangas { get; set; } = new();
}