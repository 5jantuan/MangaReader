using MangaReader.Domain.Enums;

namespace MangaReader.Domain.Entities;

public class Page
{
    public Guid Id { get; private set; }
    public Guid ChapterId { get; private set; }
    public int Number { get; private set; }
    public string ImagePath { get; private set; } = null!;
    public Chapter Chapter { get; private set; } = null!;
    private readonly List<Phrase> _phrases = new();
    public IReadOnlyCollection<Phrase> Phrases => _phrases;

    public PageProcessingStatus ProcessingStatus { get; private set; } = PageProcessingStatus.Pending;
    public DateTime? OcrProcessedAt { get; private set; }
    public DateTime? TranslationProcessedAt { get; private set; }
    public string? ProcessingError { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected Page() { }

    internal Page(Guid chapterId, int number, string imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            throw new ArgumentException("ImagePath cannot be empty");

        Id = Guid.NewGuid();
        ChapterId = chapterId;
        Number = number;
        ImagePath = imagePath;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkOcrProcessing()
    {
        ProcessingStatus = PageProcessingStatus.OcrProcessing;
        ProcessingError = null;
    }

    public void MarkOcrCompleted()
    {
        ProcessingStatus = PageProcessingStatus.OcrCompleted;
        OcrProcessedAt = DateTime.UtcNow;
        ProcessingError = null;
    }

    public void MarkOcrNeedsReview()
    {
        ProcessingStatus = PageProcessingStatus.OcrNeedsReview;
        OcrProcessedAt = DateTime.UtcNow;
        ProcessingError = null;
    }

    public void MarkTranslationProcessing()
    {
        ProcessingStatus = PageProcessingStatus.TranslationProcessing;
        ProcessingError = null;
    }

    public void MarkCompleted()
    {
        ProcessingStatus = PageProcessingStatus.Completed;
        TranslationProcessedAt = DateTime.UtcNow;
        ProcessingError = null;
    }

    public void MarkFailed(string error)
    {
        ProcessingStatus = PageProcessingStatus.Failed;
        ProcessingError = error;
    }
}
