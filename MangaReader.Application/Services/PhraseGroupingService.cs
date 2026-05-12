using MangaReader.Application.Interfaces;
using MangaReader.Application.Models;
using MangaReader.Domain.Entities;

namespace MangaReader.Application.Services;

public class PhraseGroupingService : IPhraseGroupingService
{
    public List<SpeechBubble> GroupPhrases(List<Phrase> phrases)
    {
        var sorted = phrases
            .OrderBy(p => p.Y)
            .ThenBy(p => p.X)
            .ToList();

        var bubbles = new List<SpeechBubble>();

        foreach (var phrase in sorted)
        {
            var targetBubble = bubbles.FirstOrDefault(b => IsCloseToBubble(phrase, b));

            if (targetBubble == null)
            {
                bubbles.Add(new SpeechBubble
                {
                    Phrases = new List<Phrase> { phrase }
                });
            }
            else
            {
                targetBubble.Phrases.Add(phrase);
            }
        }

        return bubbles;
    }

    private bool IsCloseToBubble(Phrase phrase, SpeechBubble bubble)
    {
        var phraseCenterX = phrase.X + phrase.Width / 2;
        var phraseCenterY = phrase.Y + phrase.Height / 2;

        var bubbleCenterX = bubble.X + bubble.Width / 2;
        var bubbleCenterY = bubble.Y + bubble.Height / 2;

        var distanceX = Math.Abs(phraseCenterX - bubbleCenterX);
        var distanceY = Math.Abs(phraseCenterY - bubbleCenterY);

        return distanceX < 160 && distanceY < 120;
    }
}