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
        var phrases = (await _phraseRepository.GetByPageIdAsync(pageId))
            .OrderBy(p => p.Y)
            .ThenBy(p => p.X)
            .ToList();

        await _bubbleRepository.RemoveByPageIdAsync(pageId);
        await _bubbleRepository.SaveChangesAsync();

        if (phrases.Count == 0)
            return;

        var groups = new List<List<Phrase>>();

        foreach (var phrase in phrases)
        {
            var targetGroup = groups.FirstOrDefault(group =>
                CanBelongToGroup(phrase, group));

            if (targetGroup == null)
            {
                groups.Add(new List<Phrase> { phrase });
            }
            else
            {
                targetGroup.Add(phrase);
            }
        }

        var number = 1;

        foreach (var group in groups)
        {
            var orderedGroup = group
                .OrderBy(p => p.Y)
                .ThenBy(p => p.X)
                .ToList();

            var x = orderedGroup.Min(p => p.X);
            var y = orderedGroup.Min(p => p.Y);
            var right = orderedGroup.Max(p => p.X + p.Width);
            var bottom = orderedGroup.Max(p => p.Y + p.Height);

            var text = string.Join(" ", orderedGroup.Select(p => p.Text));

            var bubble = new Bubble(
                pageId,
                number,
                x,
                y,
                right - x,
                bottom - y,
                text);

            foreach (var phrase in orderedGroup)
                bubble.AddPhrase(phrase);

            await _bubbleRepository.AddAsync(bubble);
            number++;
        }

        await _bubbleRepository.SaveChangesAsync();
    }

    private static bool CanBelongToGroup(Phrase phrase, List<Phrase> group)
    {
        var groupX = group.Min(p => p.X);
        var groupY = group.Min(p => p.Y);
        var groupRight = group.Max(p => p.X + p.Width);
        var groupBottom = group.Max(p => p.Y + p.Height);

        var phraseRight = phrase.X + phrase.Width;
        var phraseBottom = phrase.Y + phrase.Height;

        var verticalDistance = Math.Max(0, Math.Max(groupY - phraseBottom, phrase.Y - groupBottom));
        var horizontalDistance = Math.Max(0, Math.Max(groupX - phraseRight, phrase.X - groupRight));

        var averageHeight = group.Average(p => p.Height);

        var isCloseVertically = verticalDistance <= averageHeight * 1.5m;
        var isCloseHorizontally = horizontalDistance <= 120m;

        return isCloseVertically && isCloseHorizontally;
    }
}