namespace MangaReader.Web.ViewModels.Chapter;

public class OcrPhraseReviewViewModel
{
    public Guid PhraseId { get; set; }

    public string Text { get; set; } = string.Empty;

    public decimal X { get; set; }
    public decimal Y { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }

    public decimal Confidence { get; set; }
}