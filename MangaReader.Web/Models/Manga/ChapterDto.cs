public class ChapterDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int Number { get; set; }

    public List<string> Pages { get; set; } = new();
}