namespace MangaReader.Domain.Entities;

public class PhraseTranslation
{
    public Guid Id { get; private set; }
    
    public Guid PhraseId { get; private set; }
    public Phrase Phrase { get; private set; } = null!;
    
    public Guid LanguageId { get; private set; }
    public Language Language { get; private set; } = null!;

    public string Text { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    protected PhraseTranslation() { }

    public PhraseTranslation(Phrase phrase, Guid languageId, string text)
    {
        PhraseId = phrase.Id;

        LanguageId = languageId;

        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text cannot be empty", nameof(text));

        Id = Guid.NewGuid();
        Text = text;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text cannot be empty");

        Text = text;
    }
}
