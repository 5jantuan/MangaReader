using MangaReader.Domain.Enums;

namespace MangaReader.Web.ViewModels.Chapter;

public class ReviewOcrPageViewModel
{
    public Guid ChapterId { get; set; }
    public Guid PageId { get; set; }

    public int PageNumber { get; set; }
    public string? Translation { get; set; }
    public string ImagePath { get; set; } = string.Empty;

    public PageProcessingStatus Status { get; set; }

    public List<OcrPhraseReviewViewModel> Phrases { get; set; } = new();
    public List<SpeechBubbleReviewViewModel> Bubbles { get; set; } = new();

    
}