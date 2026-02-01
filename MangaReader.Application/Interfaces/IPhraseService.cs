using System;
using System.Threading.Tasks;
using MangaReader.Domain.Entities;

namespace MangaReader.Application.Interfaces
{
    public interface IPhraseService
    {
        Task AddTranslationAsync(Guid phraseId, Language language, string text);
    }
}