using MangaReader.Domain.Enums;

namespace MangaReader.Web.ViewModels.Chapter;

public class ReviewOcrChapterViewModel
{
    public Guid ChapterId { get; set; }
    public string ChapterTitle { get; set; } = string.Empty;

    public Guid CurrentPageId { get; set; }
    public int CurrentPageNumber { get; set; }
    public string CurrentImagePath { get; set; } = string.Empty;
    public PageProcessingStatus CurrentPageStatus { get; set; }

    public List<ReviewPageViewModel> Pages { get; set; } = new();
    public List<SpeechBubbleReviewViewModel> Bubbles { get; set; } = new();
}