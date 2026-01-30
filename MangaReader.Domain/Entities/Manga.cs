namespace MangaReader.Domain.Entities;

public class Manga
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string Author { get; private set; }
    public ICollection<MangaCover> Covers { get; private set; } = new List<MangaCover>();
    public ICollection<Chapter> Chapters { get; private set; } = new List<Chapter>();
    public DateTime CreatedAt { get; private set; }

    // Внешний ключ к пользователю
    public Guid UserId { get; private set; }
    public User User { get; private set; }

    // EF Core требует пустой конструктор, делаем его защищённым
    protected Manga() { }

    // Конструктор для создания новой манги
    public Manga(string title, string description, string author, User user)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        Id = Guid.NewGuid();
        Title = title;
        Description = description ?? string.Empty;
        Author = author ?? string.Empty;
        User = user;
        UserId = user.Id;
        CreatedAt = DateTime.UtcNow;
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
    public void AddChapter(Chapter chapter)
    {
        if (chapter == null)
            throw new ArgumentNullException(nameof(chapter));
        Chapters.Add(chapter);
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
