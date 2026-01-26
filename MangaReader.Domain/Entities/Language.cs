namespace MangaReader.Domain.Entities;

public class Language
{
    public Guid Id { get; private set; }
    public string Code { get; private set; } = null!; // например "en", "ru", "ja"
    public string Name { get; private set; } = null!; // "English", "Русский", "日本語"

    public ICollection<PhraseTranslation> PhraseTranslations { get; private set; } = new List<PhraseTranslation>();

    protected Language() { }

    public Language(string code, string name)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Language code cannot be empty");
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Language name cannot be empty");

        Id = Guid.NewGuid();
        Code = code;
        Name = name;
    }
}