namespace MangaReader.Application.Interfaces;

public interface IChapterProcessingService
{
    Task ProcessChapterAsync(Guid chapterId);
}