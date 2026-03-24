namespace MangaReader.Domain.Entities;

public class Category
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;

    private readonly List<Manga> _mangas = new();
    public IReadOnlyCollection<Manga> Mangas => _mangas;

    protected Category() { }

    public Category(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty", nameof(name));

        Id = Guid.NewGuid();
        Name = name;
    }
}