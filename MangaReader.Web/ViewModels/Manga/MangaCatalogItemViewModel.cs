namespace MangaReader.Web.ViewModels.Manga;

public class MangaCatalogItemViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? CoverUrl { get; set; }
    public int TotalViews { get; set; }
    public List<string> Categories { get; set; } = new();
}