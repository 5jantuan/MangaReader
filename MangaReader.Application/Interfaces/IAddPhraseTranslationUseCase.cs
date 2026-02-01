using MangaReader.Domain.Entities;

namespace MangaReader.Application.Interfaces;

public interface IAddPhraseTranslationUseCase
{
    Task ExecuteAsync(
        Guid phraseId,
        Language language,
        string tranlatedtext
    );
}