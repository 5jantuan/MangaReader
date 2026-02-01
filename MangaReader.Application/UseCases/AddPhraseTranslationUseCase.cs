using System.Diagnostics.SymbolStore;
using MangaReader.Application.Interfaces;
using MangaReader.Domain.Interfaces;
using MangaReader.Domain.Entities;


namespace MangaReader.Application.UseCases;


public class AddPhraseTranslationUseCase  : IAddPhraseTranslationUseCase
{
    private readonly IPhraseRepository _phraseRepository;

    public AddPhraseTranslationUseCase(IPhraseRepository phraseRepository)
    {
        _phraseRepository = phraseRepository;
    }
    
    public async Task ExecuteAsync(
        Guid phraseId,
        Language language,
        string translatedText
    )
    {
        var phrase = await _phraseRepository.GetByIdAsync(phraseId);

        if (phrase == null)
            throw new InvalidOperationException("Phrase not found");

        phrase.AddTranslation(language, translatedText);

        await _phraseRepository.SaveChangesAsync();
    }
}