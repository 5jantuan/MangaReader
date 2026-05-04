namespace MangaReader.Domain.Enums;

public enum PageProcessingStatus
{
    Pending = 0,
    OcrProcessing = 1,
    OcrCompleted = 2,
    OcrNeedsReview = 3,
    TranslationProcessing = 4,
    Completed = 5,
    Failed = 6
}