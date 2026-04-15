using MangaReader.Application.Interfaces;
using MangaReader.Domain.Interfaces;
using MangaReader.Domain.Entities;

namespace MangaReader.Application.Services;

public class ChapterProcessingService : IChapterProcessingService
{
    private readonly IChapterRepository _chapterRepository;
    private readonly IPhraseRepository _phraseRepository;
    private readonly ILanguageRepository _languageRepository;
    private readonly IOcrService _ocrService;
    private readonly ITranslationService _translationService;

    public ChapterProcessingService(
        IChapterRepository chapterRepository,
        IPhraseRepository phraseRepository,
        ILanguageRepository languageRepository,
        IOcrService ocrService,
        ITranslationService translationService)
    {
        _chapterRepository = chapterRepository;
        _phraseRepository = phraseRepository;
        _languageRepository = languageRepository;
        _ocrService = ocrService;
        _translationService = translationService;
    }

    public async Task ProcessChapterAsync(Guid chapterId)
    {
        var chapter = await _chapterRepository.GetByIdWithPagesAsync(chapterId);
        if (chapter == null)
            throw new InvalidOperationException("Chapter not found");

        var languages = await _languageRepository.GetAllAsync();
        if (languages.Count == 0)
            throw new InvalidOperationException("No languages found");

        chapter.MarkAsProcessing();
        await _chapterRepository.UpdateAsync(chapter);

        try
        {
            var pageIds = chapter.Pages.Select(p => p.Id).ToList();

            await _phraseRepository.RemoveByPageIdsAsync(pageIds);
            await _phraseRepository.SaveChangesAsync();

            // TODO: later source language should come from Manga metadata.
            // For now we temporarily assume Japanese as the source language.
            const string sourceLanguageCode = "ja";

            foreach (var page in chapter.Pages.OrderBy(p => p.Number))
            {
                var ocrPhrases = await _ocrService.ExtractPhrasesAsync(page.ImagePath);

                foreach (var ocrPhrase in ocrPhrases)
                {
                    if (string.IsNullOrWhiteSpace(ocrPhrase.Text))
                        continue;

                    if (ocrPhrase.Width <= 0 || ocrPhrase.Height <= 0)
                        continue;

                    if (ocrPhrase.Confidence < 0.40m)
                        continue;

                    var cleanedText = ocrPhrase.Text.Trim();

                    if (cleanedText.Length < 2)
                        continue;

                    var phrase = new Phrase(
                        page.Id,
                        cleanedText,
                        ocrPhrase.X,
                        ocrPhrase.Y,
                        ocrPhrase.Width,
                        ocrPhrase.Height);

                    foreach (var language in languages)
                    {
                        var translatedText = await _translationService.TranslateAsync(
                            cleanedText,
                            sourceLanguageCode,
                            language.Code);

                        phrase.AddTranslation(language, translatedText);
                    }

                    await _phraseRepository.AddAsync(phrase);
                }
            }

            await _phraseRepository.SaveChangesAsync();

            chapter.MarkAsCompleted();
            await _chapterRepository.UpdateAsync(chapter);
        }
        catch (Exception ex)
        {
            chapter.MarkAsFailed(ex.Message);
            await _chapterRepository.UpdateAsync(chapter);
            throw;
        }
    }
}