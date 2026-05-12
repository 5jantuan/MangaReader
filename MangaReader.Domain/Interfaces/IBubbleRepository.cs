using MangaReader.Domain.Entities;

namespace MangaReader.Domain.Interfaces;

public interface IBubbleRepository
{
    Task AddAsync(Bubble bubble);
    Task RemoveByPageIdAsync(Guid pageId);
    Task SaveChangesAsync();
}