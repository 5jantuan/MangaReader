namespace MangaReader.Web.Models.Manga;

public class MangaDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }

    public List<ChapterDto> Chapters { get; set; } = new();
    public string? CoverUrl { get; set; }
}