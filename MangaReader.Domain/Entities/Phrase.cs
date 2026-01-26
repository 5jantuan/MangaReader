namespace MangaReader.Domain.Entities;

public class Phrase
{
    public Guid Id { get; private set; }
    public Guid PageId { get; private set; }
    public Page Page { get; private set; }
    public string Text { get; private set; }
    public decimal X { get; private set; }
    public decimal Y { get; private set; }
    public decimal Width { get; private set; }
    public decimal Height { get; private set; }
    public ICollection<PhraseTranslation> PhraseTranslations { get; private set; } = new List<PhraseTranslation>();

    public DateTime CreatedAt { get; private set; }

    protected Phrase(){}

    public Phrase(
        Guid pageId,
        string text,
        decimal x,
        decimal y,
        decimal width,
        decimal height)
    {
        PageId = pageId;

        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text cannot be empty", nameof(text));

        if (width <= 0 || height <= 0)
            throw new ArgumentException("Width and Height must be positive");

        Id = Guid.NewGuid();
        Text = text;

        X = x;
        Y = y;
        Width = width;
        Height = height;

        CreatedAt = DateTime.UtcNow;
    }
}