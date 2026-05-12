using MangaReader.Application.Interfaces;
using MangaReader.Domain.Interfaces;
using MangaReader.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace MangaReader.Application.Services;

public class ChapterProcessingService : IChapterProcessingService
{
    private readonly IChapterRepository _chapterRepository;
    private readonly IPhraseRepository _phraseRepository;
    private readonly ILanguageRepository _languageRepository;
    private readonly IOcrService _ocrService;
    private readonly ITranslationService _translationService;
    private readonly ILogger<ChapterProcessingService> _logger;

    public ChapterProcessingService(
        IChapterRepository chapterRepository,
        IPhraseRepository phraseRepository,
        ILanguageRepository languageRepository,
        IOcrService ocrService,
        ITranslationService translationService,
        ILogger<ChapterProcessingService> logger)
    {
        _chapterRepository = chapterRepository;
        _phraseRepository = phraseRepository;
        _languageRepository = languageRepository;
        _ocrService = ocrService;
        _translationService = translationService;
        _logger = logger;
    }

    public async Task ProcessChapterAsync(Guid chapterId)
    {
        _logger.LogInformation("Starting OCR processing for chapter {ChapterId}", chapterId);

        var chapter = await _chapterRepository.GetByIdWithPagesAsync(chapterId);
        if (chapter == null)
            throw new InvalidOperationException("Chapter not found");

        if (chapter.Manga == null)
            throw new InvalidOperationException("Chapter manga was not loaded");

        if (chapter.Manga.OriginalLanguage == null)
            throw new InvalidOperationException("Manga original language was not loaded");

        var sourceLanguageCode = chapter.Manga.OriginalLanguage.Code;

        _logger.LogInformation(
            "Chapter {ChapterId} loaded with {PageCount} pages. Source language: {SourceLanguageCode}",
            chapterId,
            chapter.Pages.Count,
            sourceLanguageCode);

        chapter.MarkAsProcessing();
        await _chapterRepository.UpdateAsync(chapter);

        try
        {
            var pageIds = chapter.Pages.Select(p => p.Id).ToList();

            await _phraseRepository.RemoveByPageIdsAsync(pageIds);
            await _phraseRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Removed previous OCR phrases for chapter {ChapterId}. Page count: {PageCount}",
                chapterId,
                pageIds.Count);

            foreach (var page in chapter.Pages.OrderBy(p => p.Number))
            {
                page.MarkOcrProcessing();
                await _chapterRepository.UpdateAsync(chapter);

                _logger.LogInformation(
                    "Processing page {PageId}, number {PageNumber}, image {ImagePath}",
                    page.Id,
                    page.Number,
                    page.ImagePath);

                var ocrPhrases = await _ocrService.ExtractPhrasesAsync(
                    page.ImagePath,
                    sourceLanguageCode);

                _logger.LogInformation(
                    "OCR returned {RawPhraseCount} raw phrases for page {PageId}",
                    ocrPhrases.Count,
                    page.Id);

                var acceptedCount = 0;
                var skippedEmpty = 0;
                var skippedInvalidSize = 0;
                var skippedLowConfidence = 0;
                var skippedTooShort = 0;

                foreach (var ocrPhrase in ocrPhrases)
                {
                    if (string.IsNullOrWhiteSpace(ocrPhrase.Text))
                    {
                        skippedEmpty++;
                        continue;
                    }

                    if (ocrPhrase.Width <= 0 || ocrPhrase.Height <= 0)
                    {
                        skippedInvalidSize++;
                        continue;
                    }

                    if (ocrPhrase.Confidence < 0.40m)
                    {
                        skippedLowConfidence++;
                        continue;
                    }

                    var cleanedText = ocrPhrase.Text.Trim();

                    if (cleanedText.Length < 2)
                    {
                        skippedTooShort++;
                        continue;
                    }

                    var phrase = new Phrase(
                        page.Id,
                        cleanedText,
                        ocrPhrase.X,
                        ocrPhrase.Y,
                        ocrPhrase.Width,
                        ocrPhrase.Height,
                        ocrPhrase.Confidence);

                    await _phraseRepository.AddAsync(phrase);
                    acceptedCount++;
                }

                if (skippedLowConfidence > 0)
                    page.MarkOcrNeedsReview();
                else
                    page.MarkOcrCompleted();

                await _chapterRepository.UpdateAsync(chapter);

                _logger.LogInformation(
                    "Page {PageId} accepted {AcceptedCount} phrases. Skipped: empty={SkippedEmpty}, invalidSize={SkippedInvalidSize}, lowConfidence={SkippedLowConfidence}, tooShort={SkippedTooShort}",
                    page.Id,
                    acceptedCount,
                    skippedEmpty,
                    skippedInvalidSize,
                    skippedLowConfidence,
                    skippedTooShort);
            }

            await _phraseRepository.SaveChangesAsync();

            chapter.MarkAsCompleted();
            await _chapterRepository.UpdateAsync(chapter);

            _logger.LogInformation("Chapter {ChapterId} OCR processing completed successfully", chapterId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Chapter {ChapterId} OCR processing failed", chapterId);

            chapter.MarkAsFailed(ex.Message);
            await _chapterRepository.UpdateAsync(chapter);
            throw;
        }
    }
}
