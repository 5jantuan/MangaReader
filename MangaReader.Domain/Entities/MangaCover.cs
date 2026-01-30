namespace MangaReader.Domain.Entities;

public class MangaCover
{
    public Guid Id { get; private set; }
    public Guid MangaId { get; private set;}
    public string Path { get; private set;}
    public bool IsPinned { get; private set;}
    public DateTime CreatedAt { get; private set;}

    protected MangaCover() {}

    public MangaCover(Guid mangaId, string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Cover path cannot be empty", nameof(path));
        
        Id = Guid.NewGuid();
        MangaId = mangaId;
        Path = path;
        IsPinned = false;
        CreatedAt = DateTime.UtcNow;
    }

    internal void Pin() => IsPinned = true;
    internal void Unpin() => IsPinned = false;

}