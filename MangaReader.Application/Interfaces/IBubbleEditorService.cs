namespace MangaReader.Application.Interfaces;

public interface IBubbleEditorService
{
    Task AddManualBubbleAsync(
        Guid pageId,
        string text,
        decimal x,
        decimal y,
        decimal width,
        decimal height);

    Task UpdateBubbleAsync(
        Guid bubbleId,
        string text,
        decimal x,
        decimal y,
        decimal width,
        decimal height,
        int translationFontSize);

    Task UpdateBubblePositionAsync(
        Guid bubbleId,
        decimal x,
        decimal y,
        decimal width,
        decimal height);

    Task DeleteBubbleAsync(Guid bubbleId);

    Task DuplicateBubbleAsync(
        Guid bubbleId,
        Guid pageId);

    Task UpdateBubbleWithTranslationAsync(
        Guid bubbleId,
        string text,
        string? translation,
        decimal x,
        decimal y,
        decimal width,
        decimal height,
        int translationFontSize);

    Task MergeBubblesAsync(Guid pageId, List<Guid> bubbleIds);
}