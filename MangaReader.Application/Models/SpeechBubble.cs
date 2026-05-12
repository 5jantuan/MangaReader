using MangaReader.Domain.Entities;

namespace MangaReader.Application.Models;

public class SpeechBubble
{
    public List<Phrase> Phrases { get; set; } = new();
    public string? Translation { get; set; }

    public string CombinedText =>
        string.Join(" ", Phrases
            .OrderBy(p => p.Y)
            .ThenBy(p => p.X)
            .Select(p => p.Text));

    public decimal X => Phrases.Min(p => p.X);
    public decimal Y => Phrases.Min(p => p.Y);

    public decimal Width =>
        Phrases.Max(p => p.X + p.Width) - X;

    public decimal Height =>
        Phrases.Max(p => p.Y + p.Height) - Y;
}