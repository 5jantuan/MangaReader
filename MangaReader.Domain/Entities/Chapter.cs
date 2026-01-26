namespace MangaReader.Domain.Entities;

public class Chapter
{
    public Guid Id { get; private set; }
    public Guid MangaId { get; private set; }
    public Manga Manga { get; private set; }

    public int Number { get; private set; }
    public string Title { get; private set; }

    public ICollection<Page> Pages { get; private set; } = new List<Page>();
    public DateTime CreatedAt { get; private set; }

    protected Chapter() { } // для EF

    // Конструктор с объектом Manga
    public Chapter(Manga manga, int number, string title)
    {
        Manga = manga ?? throw new ArgumentNullException(nameof(manga));
        MangaId = manga.Id;

        Number = number > 0 ? number : throw new ArgumentException("Chapter number must be positive", nameof(number));
        Title = !string.IsNullOrWhiteSpace(title) ? title : throw new ArgumentException("Title cannot be empty", nameof(title));

        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateInfo(int number, string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        if (number <= 0)
            throw new ArgumentException("Chapter number must be positive", nameof(number));

        Number = number;
        Title = title;
    }

    public void AddPage(Page page)
    {
        if (page == null)
            throw new ArgumentNullException(nameof(page));
        Pages.Add(page);
    }
}
