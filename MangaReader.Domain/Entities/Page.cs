namespace MangaReader.Domain.Entities;

public class Page
{
    public Guid Id {get; private set;}
    public Guid ChapterId{ get; private set;}
    public int Number { get; private set;}
    public string ImagePath { get; set; } = null!;
    public Chapter Chapter { get; set; } = null!;

    public ICollection<Phrase> Phrases { get; set; } = new List<Phrase>();
    public DateTime CreatedAt{get; private set;}

    protected Page(Guid chapterId, int number, string imagePath)
    {
        Id = Guid.NewGuid();
        ChapterId = chapterId;
        Number = number;
        ImagePath = !string.IsNullOrWhiteSpace(imagePath) ? imagePath : throw new ArgumentException("ImagePath cannot be empty");
        CreatedAt = DateTime.UtcNow;
    }
}