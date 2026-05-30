namespace MangaReader.Application.Interfaces;

public interface IOcrEditorService
{
    Task AddManualPhraseAsync(
        Guid pageId,
        string text,
        decimal x,
        decimal y,
        decimal width,
        decimal height);

    Task UpdatePhraseAsync(
        Guid phraseId,
        string text,
        Guid pageId);

    Task DeletePhraseAsync(
        Guid phraseId,
        Guid pageId);

    Task UpdateBubbleAsPhraseEditAsync(
        Guid bubbleId,
        Guid pageId,
        string text,
        decimal x,
        decimal y,
        decimal width,
        decimal height,
        int translationFontSize);
}