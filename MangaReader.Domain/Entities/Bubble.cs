namespace MangaReader.Domain.Entities;

public class Bubble
{
    public Guid Id { get; private set; }

    public Guid PageId { get; private set; }
    public Page Page { get; private set; } = null!;

    public int Number { get; private set; }

    public decimal X { get; private set; }
    public decimal Y { get; private set; }
    public decimal Width { get; private set; }
    public decimal Height { get; private set; }

    public string OriginalText { get; private set; } = null!;

    private readonly List<Phrase> _phrases = new();
    public IReadOnlyCollection<Phrase> Phrases => _phrases;

    private readonly List<BubbleTranslation> _translations = new();
    public IReadOnlyCollection<BubbleTranslation> Translations => _translations;

    public DateTime CreatedAt { get; private set; }

    protected Bubble() { }

    public Bubble(
        Guid pageId,
        int number,
        decimal x,
        decimal y,
        decimal width,
        decimal height,
        string originalText)
    {
        if (string.IsNullOrWhiteSpace(originalText))
            throw new ArgumentException("Original text cannot be empty", nameof(originalText));

        if (width <= 0 || height <= 0)
            throw new ArgumentException("Width and Height must be positive");

        Id = Guid.NewGuid();
        PageId = pageId;
        Number = number;
        X = x;
        Y = y;
        Width = width;
        Height = height;
        OriginalText = originalText;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddPhrase(Phrase phrase)
    {
        if (phrase.PageId != PageId)
            throw new InvalidOperationException("Phrase must belong to the same page");

        phrase.AssignToBubble(Id);
        _phrases.Add(phrase);
    }

    public void AddTranslation(Guid languageId, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Translation text cannot be empty", nameof(text));

        if (_translations.Any(t => t.LanguageId == languageId))
            throw new InvalidOperationException("Translation for this language already exists");

        _translations.Add(new BubbleTranslation(Id, languageId, text));
    }
}