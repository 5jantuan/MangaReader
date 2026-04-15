using MangaReader.Application.DTOs;

namespace MangaReader.Application.Interfaces;

public interface IOcrService
{
    Task<List<OcrPhraseDto>> ExtractPhrasesAsync(string imagePath);
}