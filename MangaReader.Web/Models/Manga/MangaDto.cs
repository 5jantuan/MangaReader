namespace MangaReader.Web.Models.Manga;
public class MangaDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? CoverUrl { get; set; }
    public int TotalViews { get; set; }

    public List<ChapterDto> Chapters { get; set; } = new();
    public List<string> Categories { get; set; } = new();
}

