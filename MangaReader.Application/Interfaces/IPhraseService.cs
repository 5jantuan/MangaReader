using System;
using System.Threading.Tasks;

namespace MangaReader.Application.Interfaces
{
    public interface IPhraseService
    {
        Task AddTranslationAsync(Guid phraseId, Guid languageId, string text);
    }
}