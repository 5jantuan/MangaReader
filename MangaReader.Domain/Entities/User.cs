namespace MangaReader.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string UserName { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // навигация
    public ICollection<Manga> Mangas { get; private set; } = new List<Manga>();

    protected User() { } // для EF

    public User(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Username cannot be empty", nameof(userName));

        Id = Guid.NewGuid();
        UserName = userName;
        CreatedAt = DateTime.UtcNow;
    }
}
