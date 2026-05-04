using System.Threading.Channels;
using MangaReader.Application.Interfaces;

namespace MangaReader.Infrastructure.Services;

public class ChapterProcessingQueue : IChapterProcessingQueue
{
    private readonly Channel<Guid> _queue;

    public ChapterProcessingQueue()
    {
        _queue = Channel.CreateUnbounded<Guid>();
    }

    public async ValueTask QueueChapterAsync(Guid chapterId)
    {
        await _queue.Writer.WriteAsync(chapterId);
    }

    public async ValueTask<Guid> DequeueChapterAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}