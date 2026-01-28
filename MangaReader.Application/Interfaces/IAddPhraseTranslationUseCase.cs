public interface AddTranslationUseCase
{
    Task ExecuteAsync(
        Guid phraseId,
        Guid languageId,
        string tranlatedtext
    );
}