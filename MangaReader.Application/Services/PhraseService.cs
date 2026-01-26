using MangaReader.Domain.Interfaces;
using MangaReader.Application.Interfaces;

namespace MangaReader.Application.Services
{
    public class PhraseService : IPhraseService
    {
        private readonly IPhraseRepository _phraseRepository;

        public PhraseService(IPhraseRepository phraseRepository)
        {
            _phraseRepository = phraseRepository;
        }

        public async Task AddTranslationAsync(Guid phraseId, Guid languageId, string text)
        {
            var phrase = await _phraseRepository.GetByIdAsync(phraseId);
            if (phrase == null)
                throw new InvalidOperationException("Phrase not found");

            phrase.AddTranslation(languageId, text);
            await _phraseRepository.UpdateAsync(phrase);
        }
    }
}
