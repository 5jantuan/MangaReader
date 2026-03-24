namespace MangaReader.Web.Models.Manga;

public class ChapterDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Number { get; set; }
    public int Views { get; set; }
    public List<string> Pages { get; set; } = new();
}