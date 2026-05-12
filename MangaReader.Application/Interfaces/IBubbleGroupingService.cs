namespace MangaReader.Application.Interfaces;

public interface IBubbleGroupingService
{
    Task GroupPageAsync(Guid pageId);
}