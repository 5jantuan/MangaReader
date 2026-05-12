using MangaReader.Application.Models;
using MangaReader.Domain.Entities;

namespace MangaReader.Application.Interfaces;

public interface IPhraseGroupingService
{
    List<SpeechBubble> GroupPhrases(List<Phrase> phrases);
}