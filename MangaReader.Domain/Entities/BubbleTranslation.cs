namespace MangaReader.Domain.Entities;

public class BubbleTranslation
{
    public Guid Id { get; private set; }

    public Guid BubbleId { get; private set; }
    public Bubble Bubble { get; private set; } = null!;

    public Guid LanguageId { get; private set; }
    public Language Language { get; private set; } = null!;

    public string Text { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    protected BubbleTranslation() { }

    public BubbleTranslation(Guid bubbleId, Guid languageId, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text cannot be empty", nameof(text));

        Id = Guid.NewGuid();
        BubbleId = bubbleId;
        LanguageId = languageId;
        Text = text;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text cannot be empty", nameof(text));

        Text = text;
    }
}