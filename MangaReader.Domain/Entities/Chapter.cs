namespace MangaReader.Domain.Entities;

public class Chapter
{
    public Guid Id { get; private set; }
    public Guid MangaId { get; private set; }
    public Manga Manga { get; private set; } = null!;
    public int Number { get; private set; }
    public string Title { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<Page> _pages = new();
    public IReadOnlyCollection<Page> Pages => _pages;

    protected Chapter() { }

    internal Chapter(Manga manga, int number, string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty");

        Id = Guid.NewGuid();
        Manga = manga;
        MangaId = manga.Id;
        Number = number;
        Title = title;
        CreatedAt = DateTime.UtcNow;
    }

    internal void AddPage(string imagePath)
    {
        var nextNumber = _pages.Count + 1;
        var page = new Page(Id, nextNumber, imagePath);
        _pages.Add(page);
    }

    public void Rename(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty");

        Title = title;
    }
}
