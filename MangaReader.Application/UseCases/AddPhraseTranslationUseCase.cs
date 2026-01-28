using MangaReader.Domain.Interfaces;

public class AddPhraseTranslationUseCase : IAddPhraseTranslationUseCase
{
    private readonly IPhraseRepository _phraseRepjsitory;

    public async Task ExecuteAsync(
        Guid phraseId,
        Guid languageId,
        string translatedText
    )
    {
        var phrase = await _phraseRepository.GetByIdAsync(phraseId);

        if (phrase == null)
            throw new InvalidOperationException("Phrase not found");

        phrase.AddTranslation(languageId, translatedText);

        await _phraseRepository.SaveChangesAsync();
    }
}