using MangaReader.Domain.Entities;

namespace MangaReader.Domain.Interfaces;

public interface IPhraseRepository
{
    Task<Phrase?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Phrase>> GetByPageIdAsync(Guid pageId);
    Task AddAsync(Phrase phrase);
    Task RemoveAsync(Phrase phrase);
    Task UpdateAsync(Phrase phrase);
}