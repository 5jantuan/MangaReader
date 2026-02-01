namespace MangaReader.Domain.Entities;

public class Manga
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public Guid AuthorId { get; private set; }
    private readonly List<MangaCover> _covers;
    private readonly List<Chapter> _chapters;

    public IReadOnlyCollection<MangaCover> Covers => _covers;
    public IReadOnlyCollection<Chapter> Chapters => _chapters;

    public DateTime CreatedAt { get; private set; }

    // EF Core требует пустой конструктор, делаем его защищённым
    protected Manga()
    {
        _covers = new();
        _chapters = new();
    }

    // Конструктор для создания новой манги
    public Manga(string title, string description, User author)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        if (!author.IsAuthor())
            throw new InvalidOperationException("User must have Author role");

        Id = Guid.NewGuid();
        AuthorId = author.Id;
        Title = title;
        Description = description ?? string.Empty;
        CreatedAt = DateTime.UtcNow;
        _covers = new();
        _chapters = new();
    }

    // Метод для обновления информации
    public void UpdateInfo(string title, string description)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        Title = title;
        Description = description ?? string.Empty;
    }

    // Метод для добавления главы
    public Chapter CreateChapter(string title, IEnumerable<string> pageImagePaths)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Chapter title cannot be empty");

        var nextNumber = Chapters.Count + 1;

        var chapter = new Chapter(this, nextNumber, title);

        foreach (var path in pageImagePaths)
        chapter.AddPage(path);

        _chapters.Add(chapter);
        return chapter;
    }

    public void PinCover(Guid coverId)
    {
        var coverToPin = Covers.FirstOrDefault(c => c.Id == coverId)
            ?? throw new InvalidOperationException("Cover not found");

        if (coverToPin.IsPinned)
            return;

        foreach (var cover in Covers.Where(c => c.IsPinned))
            cover.Unpin();

        coverToPin.Pin();
    }

}
