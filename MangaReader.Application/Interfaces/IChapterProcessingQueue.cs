namespace MangaReader.Application.Interfaces;

public interface IChapterProcessingQueue
{
    ValueTask QueueChapterAsync(Guid chapterId);
    ValueTask<Guid> DequeueChapterAsync(CancellationToken cancellationToken);
}