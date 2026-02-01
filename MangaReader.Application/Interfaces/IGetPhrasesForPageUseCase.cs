using MangaReader.Domain.Entities;

namespace MangaReader.Application.Interfaces;

public interface IGetPhrasesForPageUseCase
{
    Task<IReadOnlyList<Phrase>> ExecuteAsync(Guid pageId);
}
