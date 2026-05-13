namespace MangaReader.Domain.Enums;

public enum PageProcessingStatus
{
    Pending = 0,
    OcrProcessing = 1,
    OcrCompleted = 2,
    OcrNeedsReview = 3,
    BubbleGroupingRequired = 4,
    TranslationProcessing = 5,
    Completed = 6,
    Failed = 7
}