namespace MangaReader.Application.Interfaces;

public interface ITranslationService
{
    Task<string> TranslateAsync(string text, string sourceLanguageCode, string targetLanguageCode);
}