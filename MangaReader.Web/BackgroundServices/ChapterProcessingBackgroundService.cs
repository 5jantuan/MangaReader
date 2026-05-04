using MangaReader.Application.Interfaces;

namespace MangaReader.Web.BackgroundServices;

public class ChapterProcessingBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IChapterProcessingQueue _queue;
    private readonly ILogger<ChapterProcessingBackgroundService> _logger;

    public ChapterProcessingBackgroundService(
        IServiceScopeFactory scopeFactory,
        IChapterProcessingQueue queue,
        ILogger<ChapterProcessingBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _queue = queue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Chapter processing background service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var chapterId = await _queue.DequeueChapterAsync(stoppingToken);

                _logger.LogInformation("Dequeued chapter {ChapterId} for background processing", chapterId);

                using var scope = _scopeFactory.CreateScope();

                var chapterProcessingService = scope.ServiceProvider
                    .GetRequiredService<IChapterProcessingService>();

                await chapterProcessingService.ProcessChapterAsync(chapterId);

                _logger.LogInformation("Background processing completed for chapter {ChapterId}", chapterId);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Background chapter processing failed");
            }
        }

        _logger.LogInformation("Chapter processing background service stopped");
    }
}