using MangaReader.Domain.Entities;
using MangaReader.Domain.Interfaces;
using MangaReader.Application.Interfaces;

namespace MangaReader.Application.UseCases;

public class GetPhrasesForPageUseCase : IGetPhrasesForPageUseCase
{
    private readonly IPhraseRepository _phraseRepository;

    public GetPhrasesForPageUseCase(IPhraseRepository phraseRepository)
    {
        _phraseRepository = phraseRepository;
    }

    public async Task<IReadOnlyList<Phrase>> ExecuteAsync(Guid pageId)
    {
        return await _phraseRepository.GetByPageIdAsync(pageId);
    }
}
