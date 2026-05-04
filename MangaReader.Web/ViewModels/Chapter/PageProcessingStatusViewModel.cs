using MangaReader.Domain.Enums;

namespace MangaReader.Web.ViewModels.Chapter;

public class PageProcessingStatusViewModel
{
    public Guid PageId { get; set; }

    public int Number { get; set; }

    public string ImagePath { get; set; } = string.Empty;

    public PageProcessingStatus Status { get; set; }
}