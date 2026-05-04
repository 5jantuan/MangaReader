using MangaReader.Domain.Enums;

namespace MangaReader.Web.ViewModels.Chapter;

public class ChapterProcessingStatusViewModel
{
    public Guid ChapterId { get; set; }

    public string ChapterTitle { get; set; } = string.Empty;

    public ProcessingStatus ChapterStatus { get; set; }

    public int TotalPages { get; set; }

    public int CompletedPages { get; set; }

    public List<PageProcessingStatusViewModel> Pages { get; set; } = new();

    public int ProgressPercent =>
        TotalPages == 0 ? 0 : (int)Math.Round((double)CompletedPages / TotalPages * 100);
}