namespace MangaReader.Web.ViewModels.Chapter;

public class SpeechBubbleReviewViewModel
{
    public Guid BubbleId { get; set; }
    public string Text { get; set; } = string.Empty;

    public string? Translation { get; set; }
    public int TranslationFontSize { get; set; } = 14;

    public decimal X { get; set; }
    public decimal Y { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }

    public List<Guid> PhraseIds { get; set; } = new();
}