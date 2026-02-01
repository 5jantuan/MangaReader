namespace MangaReader.Domain.Entities;

public class Page
{
    public Guid Id { get; private set; }
    public Guid ChapterId { get; private set; }
    public int Number { get; private set; }
    public string ImagePath { get; private set; }
    public Chapter Chapter { get; private set; } = null!;
    private readonly List<Phrase> _phrases = new();
    public IReadOnlyCollection<Phrase> Phrases => _phrases;
    public DateTime CreatedAt { get; private set; }

    protected Page() { }

    internal Page(Guid chapterId, int number, string imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            throw new ArgumentException("ImagePath cannot be empty");

        Id = Guid.NewGuid();
        ChapterId = chapterId;
        Number = number;
        ImagePath = imagePath;
        CreatedAt = DateTime.UtcNow;
    }
}
