using MangaReader.Domain.Entities;
using MangaReader.Application.Interfaces;
using MangaReader.Domain.Interfaces;


namespace MangaReader.Application.Services;

public class OcrEditorService : IOcrEditorService
{
    private readonly IPhraseRepository _phraseRepository;
    private readonly IBubbleRepository _bubbleRepository;
    private readonly IChapterRepository _chapterRepository;

    public OcrEditorService(
        IPhraseRepository phraseRepository,
        IBubbleRepository bubbleRepository,
        IChapterRepository chapterRepository)
    {
        _phraseRepository = phraseRepository;
        _bubbleRepository = bubbleRepository;
        _chapterRepository = chapterRepository;
    }

    public async Task AddManualPhraseAsync(
        Guid pageId,
        string text,
        decimal x,
        decimal y,
        decimal width,
        decimal height)
    {
        var phrase = new Phrase(
            pageId,
            text,
            x,
            y,
            width,
            height,
            1m);

        await _phraseRepository.AddAsync(phrase);

        await ResetPageAsync(pageId);

        await _phraseRepository.SaveChangesAsync();
    }

    public async Task UpdatePhraseAsync(
        Guid phraseId,
        string text,
        Guid pageId)
    {
        var phrase = await _phraseRepository.GetByIdAsync(phraseId);

        if (phrase == null)
            throw new InvalidOperationException("Phrase not found");

        phrase.UpdateText(text);

        await ResetPageAsync(pageId);

        await _phraseRepository.SaveChangesAsync();
    }

    public async Task DeletePhraseAsync(
        Guid phraseId,
        Guid pageId)
    {
        var phrase = await _phraseRepository.GetByIdAsync(phraseId);

        if (phrase == null)
            throw new InvalidOperationException("Phrase not found");

        await _phraseRepository.RemoveAsync(phrase);

        await ResetPageAsync(pageId);

        await _phraseRepository.SaveChangesAsync();
    }

    public async Task UpdateBubbleAsPhraseEditAsync(
        Guid bubbleId,
        Guid pageId,
        string text,
        decimal x,
        decimal y,
        decimal width,
        decimal height,
        int translationFontSize)
    {
        var bubble = await _bubbleRepository.GetByIdAsync(bubbleId);

        if (bubble == null)
            throw new InvalidOperationException("Bubble not found");

        bubble.UpdateTextAndBox(text, x, y, width, height, translationFontSize);

        await _bubbleRepository.RemoveTranslationsByBubbleIdAsync(bubbleId);

        await ResetPageAsync(pageId);

        await _bubbleRepository.SaveChangesAsync();
    }

    private async Task ResetPageAsync(Guid pageId)
    {
        var page = await _chapterRepository.GetPageWithPhrasesAsync(pageId);

        if (page == null)
            throw new InvalidOperationException("Page not found");

        page.MarkBubbleGroupingRequired();

        await _chapterRepository.UpdatePageAsync(page);
    }
}