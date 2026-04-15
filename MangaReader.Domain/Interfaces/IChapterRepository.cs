using MangaReader.Domain.Entities;

namespace MangaReader.Domain.Interfaces;

public interface IChapterRepository
{
    Task<Chapter?> GetByIdWithPagesAsync(Guid chapterId);
    Task UpdateAsync(Chapter chapter);
}