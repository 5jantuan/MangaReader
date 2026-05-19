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
    private readonly IBubbleGroupingService _bubbleGroupingService;

    public ChapterProcessingService(
        IChapterRepository chapterRepository,
        IPhraseRepository phraseRepository,
        ILanguageRepository languageRepository,
        IOcrService ocrService,
        ITranslationService translationService,
        IBubbleGroupingService bubbleGroupingService,
        ILogger<ChapterProcessingService> logger)
    {
        _chapterRepository = chapterRepository;
        _phraseRepository = phraseRepository;
        _languageRepository = languageRepository;
        _ocrService = ocrService;
        _translationService = translationService;
        _logger = logger;
        _bubbleGroupingService = bubbleGroupingService;
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

        chapter.MarkAsProcessing();
        await _chapterRepository.UpdateAsync(chapter);

        try
        {
            var pageIds = chapter.Pages.Select(p => p.Id).ToList();

            await _phraseRepository.RemoveByPageIdsAsync(pageIds);
            await _phraseRepository.SaveChangesAsync();

            foreach (var page in chapter.Pages.OrderBy(p => p.Number))
            {
                page.MarkOcrProcessing();
                await _chapterRepository.UpdateAsync(chapter);

                var ocrPhrases = await _ocrService.ExtractPhrasesAsync(
                    page.ImagePath,
                    sourceLanguageCode);

                var acceptedCount = 0;
                var skippedLowConfidence = 0;

                foreach (var ocrPhrase in ocrPhrases)
                {
                    if (string.IsNullOrWhiteSpace(ocrPhrase.Text))
                        continue;

                    if (ocrPhrase.Width <= 0 || ocrPhrase.Height <= 0)
                        continue;

                    if (ocrPhrase.Confidence < 0.40m)
                    {
                        skippedLowConfidence++;
                        continue;
                    }

                    var cleanedText = ocrPhrase.Text.Trim();

                    if (cleanedText.Length < 2)
                        continue;

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

                await _phraseRepository.SaveChangesAsync();

                await _bubbleGroupingService.GroupPageAsync(page.Id);

                if (skippedLowConfidence > 0)
                    page.MarkOcrNeedsReview();
                else
                    page.MarkOcrCompleted();

                await _chapterRepository.UpdateAsync(chapter);

                _logger.LogInformation(
                    "Page {PageId} accepted {AcceptedCount} phrases",
                    page.Id,
                    acceptedCount);
            }

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
