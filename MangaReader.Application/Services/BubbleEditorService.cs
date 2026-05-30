using MangaReader.Domain.Entities;
using MangaReader.Domain.Interfaces;
using MangaReader.Application.Interfaces;


namespace MangaReader.Application.Services;

public class BubbleEditorService : IBubbleEditorService
{
    private readonly IBubbleRepository _bubbleRepository;

    public BubbleEditorService(IBubbleRepository bubbleRepository)
    {
        _bubbleRepository = bubbleRepository;
    }

    // ===================== CREATE =====================
    public async Task AddManualBubbleAsync(
        Guid pageId,
        string text,
        decimal x,
        decimal y,
        decimal width,
        decimal height)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text cannot be empty");

        var bubbles = await _bubbleRepository.GetByPageIdAsync(pageId);

        var nextNumber = bubbles.Any()
            ? bubbles.Max(b => b.Number) + 1
            : 1;

        var bubble = new Bubble(
            pageId,
            nextNumber,
            x,
            y,
            width,
            height,
            text);

        await _bubbleRepository.AddAsync(bubble);
        await _bubbleRepository.SaveChangesAsync();
    }

    // ===================== UPDATE FULL =====================
    public async Task UpdateBubbleAsync(
        Guid bubbleId,
        string text,
        decimal x,
        decimal y,
        decimal width,
        decimal height,
        int translationFontSize)
    {
        var bubble = await _bubbleRepository.GetByIdAsync(bubbleId)
            ?? throw new InvalidOperationException("Bubble not found");

        bubble.UpdateTextAndBox(text, x, y, width, height, translationFontSize);

        await _bubbleRepository.UpdateAsync(bubble);
        await _bubbleRepository.SaveChangesAsync();
    }

    // ===================== MOVE ONLY =====================
    public async Task UpdateBubblePositionAsync(
        Guid bubbleId,
        decimal x,
        decimal y,
        decimal width,
        decimal height)
    {
        var bubble = await _bubbleRepository.GetByIdAsync(bubbleId)
            ?? throw new InvalidOperationException("Bubble not found");

        bubble.UpdateTextAndBox(
            bubble.OriginalText,
            x,
            y,
            width,
            height,
            bubble.TranslationFontSize);

        await _bubbleRepository.UpdateAsync(bubble);
        await _bubbleRepository.SaveChangesAsync();
    }

    // ===================== DELETE =====================
    public async Task DeleteBubbleAsync(Guid bubbleId)
    {
        var bubble = await _bubbleRepository.GetByIdAsync(bubbleId);

        if (bubble == null)
            return;

        await _bubbleRepository.RemoveAsync(bubble);
        await _bubbleRepository.SaveChangesAsync();
    }

    // ===================== DUPLICATE =====================
    public async Task DuplicateBubbleAsync(Guid bubbleId, Guid pageId)
    {
        var bubble = await _bubbleRepository.GetByIdAsync(bubbleId)
            ?? throw new InvalidOperationException("Bubble not found");

        var bubbles = await _bubbleRepository.GetByPageIdAsync(pageId);

        var nextNumber = bubbles.Any()
            ? bubbles.Max(b => b.Number) + 1
            : 1;

        var copy = new Bubble(
            pageId,
            nextNumber,
            bubble.X + 20,
            bubble.Y + 20,
            bubble.Width,
            bubble.Height,
            bubble.OriginalText);

        await _bubbleRepository.AddAsync(copy);
        await _bubbleRepository.SaveChangesAsync();
    }

    // ===================== MERGE =====================
    public async Task MergeBubblesAsync(Guid pageId, List<Guid> bubbleIds)
    {
        if (bubbleIds == null || bubbleIds.Count < 2)
            return;

        var bubbles = new List<Bubble>();

        foreach (var id in bubbleIds)
        {
            var bubble = await _bubbleRepository.GetByIdAsync(id);
            if (bubble != null)
                bubbles.Add(bubble);
        }

        if (bubbles.Count < 2)
            return;

        var ordered = bubbles
            .OrderBy(b => b.Y)
            .ThenBy(b => b.X)
            .ToList();

        var x = ordered.Min(b => b.X);
        var y = ordered.Min(b => b.Y);
        var right = ordered.Max(b => b.X + b.Width);
        var bottom = ordered.Max(b => b.Y + b.Height);

        var text = string.Join(" ", ordered.Select(b => b.OriginalText));

        var existing = await _bubbleRepository.GetByPageIdAsync(pageId);

        var nextNumber = existing.Any()
            ? existing.Max(b => b.Number) + 1
            : 1;

        var merged = new Bubble(
            pageId,
            nextNumber,
            x,
            y,
            right - x,
            bottom - y,
            text
        );

        foreach (var b in bubbles)
        {
            await _bubbleRepository.RemoveAsync(b);
        }

        await _bubbleRepository.AddAsync(merged);
        await _bubbleRepository.SaveChangesAsync();
    }

    public async Task UpdateBubbleWithTranslationAsync(
        Guid bubbleId,
        string text,
        string? translation,
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

        if (!string.IsNullOrWhiteSpace(translation))
        {
            var ru = bubble.Translations
                .FirstOrDefault(t => t.Language.Code == "ru");

            if (ru != null)
                ru.UpdateText(translation);
            else
            {
                // упрощённо
                var translationEntity = new BubbleTranslation(
                    bubble.Id,
                    Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    translation
                );

                await _bubbleRepository.AddTranslationAsync(translationEntity);
            }
        }

        await _bubbleRepository.UpdateAsync(bubble);
        await _bubbleRepository.SaveChangesAsync();
    }
}