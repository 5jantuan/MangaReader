using MangaReader.Application.Interfaces;
using MangaReader.Domain.Entities;
using MangaReader.Domain.Interfaces;

namespace MangaReader.Application.Services;

public class BubbleGroupingService : IBubbleGroupingService
{
    private readonly IPhraseRepository _phraseRepository;
    private readonly IBubbleRepository _bubbleRepository;

    public BubbleGroupingService(
        IPhraseRepository phraseRepository,
        IBubbleRepository bubbleRepository)
    {
        _phraseRepository = phraseRepository;
        _bubbleRepository = bubbleRepository;
    }

    public async Task GroupPageAsync(Guid pageId)
    {
        // 1. получаем ВСЕ фразы страницы (источник истины)
        var phrases = await _phraseRepository.GetByPageIdAsync(pageId);

        if (phrases.Count == 0)
        {
            // если фраз нет → просто чистим bubbles
            await _bubbleRepository.RemoveByPageIdAsync(pageId);
            await _bubbleRepository.SaveChangesAsync();
            return;
        }

        // 2. ВСЕГДА полностью пересоздаём bubbles
        await _bubbleRepository.RemoveByPageIdAsync(pageId);
        await _bubbleRepository.SaveChangesAsync();

        // 3. сортировка как входной поток
        var ordered = phrases
            .OrderBy(p => p.Y)
            .ThenBy(p => p.X)
            .ToList();

        // 4. группировка
        var groups = BuildGroups(ordered);

        // 5. создание bubbles (чистый builder)
        var number = 1;

        foreach (var group in groups)
        {
            var bubble = BuildBubble(pageId, group, number);

            await _bubbleRepository.AddAsync(bubble);

            number++;
        }

        await _bubbleRepository.SaveChangesAsync();
    }

    // -------------------------
    // PURE BUILDER METHODS
    // -------------------------

    private static List<List<Phrase>> BuildGroups(List<Phrase> phrases)
    {
        var groups = new List<List<Phrase>>();

        foreach (var phrase in phrases)
        {
            var group = groups.FirstOrDefault(g => CanBelongToGroup(phrase, g));

            if (group == null)
                groups.Add(new List<Phrase> { phrase });
            else
                group.Add(phrase);
        }

        return groups;
    }

    private static Bubble BuildBubble(Guid pageId, List<Phrase> group, int number)
    {
        var ordered = group
            .OrderBy(p => p.Y)
            .ThenBy(p => p.X)
            .ToList();

        var x = ordered.Min(p => p.X);
        var y = ordered.Min(p => p.Y);
        var right = ordered.Max(p => p.X + p.Width);
        var bottom = ordered.Max(p => p.Y + p.Height);

        var text = string.Join(" ", ordered.Select(p => p.Text));

        var bubble = new Bubble(
            pageId,
            number,
            x,
            y,
            right - x,
            bottom - y,
            text
        );

        foreach (var phrase in ordered)
            bubble.AddPhrase(phrase);

        return bubble;
    }

    private static bool CanBelongToGroup(Phrase phrase, List<Phrase> group)
    {
        var groupX = group.Min(p => p.X);
        var groupY = group.Min(p => p.Y);
        var groupRight = group.Max(p => p.X + p.Width);
        var groupBottom = group.Max(p => p.Y + p.Height);

        var phraseRight = phrase.X + phrase.Width;
        var phraseBottom = phrase.Y + phrase.Height;

        var verticalDistance =
            Math.Max(0, Math.Max(groupY - phraseBottom, phrase.Y - groupBottom));

        var horizontalDistance =
            Math.Max(0, Math.Max(groupX - phraseRight, phrase.X - groupRight));

        var averageHeight = group.Average(p => p.Height);

        return verticalDistance <= averageHeight * 1.5m
            && horizontalDistance <= 120m;
    }
}