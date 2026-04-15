using MangaReader.Application.Interfaces;

namespace MangaReader.Infrastructure.Translation;

public class FakeTranslationService : ITranslationService
{
    public Task<string> TranslateAsync(string text, string sourceLanguageCode, string targetLanguageCode)
    {
        var translated = $"[{targetLanguageCode}] {text}";
        return Task.FromResult(translated);
    }
}