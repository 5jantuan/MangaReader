using System.Diagnostics.SymbolStore;
using System.Linq.Expressions;
using System.Text;

namespace MangaReader.Domain.Entities;

public class Phrase
{
    public Guid Id { get; private set; }
    public Guid PageId { get; private set; }
    public Page Page { get; private set; } = null!;
    public string Text { get; private set; } = null!;
    public decimal X { get; private set; }
    public decimal Y { get; private set; }
    public decimal Width { get; private set; }
    public decimal Height { get; private set; }
    public decimal Confidence { get; private set; }
    private readonly List<PhraseTranslation> _translations = new();
    public IReadOnlyCollection<PhraseTranslation> PhraseTranslations => _translations;

    public DateTime CreatedAt { get; private set; }

    protected Phrase(){}

    public Phrase(
        Guid pageId,
        string text,
        decimal x,
        decimal y,
        decimal width,
        decimal height,
        decimal confidence)
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
        if (confidence < 0 || confidence > 1)
            throw new ArgumentException("Confidence must be between 0 and 1");

        Confidence = confidence;

        CreatedAt = DateTime.UtcNow;
    }

    public void AddTranslation(Language language, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Translation text cannot be empty", nameof(text));
        }
            
        if(_translations.Any(t => t.Language.Id == language.Id))
        {
            throw new InvalidOperationException("Translation for this language already exists");
        }

        var translation = new PhraseTranslation(
            this,
            language,
            text
        );

        _translations.Add(translation);
    }
    
    public void UpdateText(string newText)
    {
        if (string.IsNullOrWhiteSpace(newText))
            throw new ArgumentException("Text cannot be empty", nameof(newText));

        Text = newText;
}
}