namespace MangaReader.Web.ViewModels.Chapter;

public class CreateChapterViewModel
{
    public Guid MangaId { get; set; }
    public string Title { get; set; }
    public int Number { get; set; }
}