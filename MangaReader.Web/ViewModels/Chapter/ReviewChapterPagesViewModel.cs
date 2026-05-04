namespace MangaReader.Web.ViewModels.Chapter;

public class ReviewChapterPagesViewModel
{
    public Guid ChapterId { get; set; }

    public string ChapterTitle { get; set; } = string.Empty;

    public int ChapterNumber { get; set; }

    public List<ReviewPageViewModel> Pages { get; set; } = new();
}