namespace MangaReader.Web.ViewModels.Chapter;

public class ReviewPageViewModel
{
    public Guid PageId { get; set; }

    public int Number { get; set; }

    public string ImagePath { get; set; } = string.Empty;
}