using MangaReader.Application.DTOs;
using MangaReader.Application.Interfaces;

namespace MangaReader.Infrastructure.Ocr;

public class FakeOcrService : IOcrService
{
    public Task<List<OcrPhraseDto>> ExtractPhrasesAsync(string imagePath)
    {
        var phrases = new List<OcrPhraseDto>
        {
            new OcrPhraseDto
            {
                Text = "Hello!",
                X = 100,
                Y = 120,
                Width = 180,
                Height = 50
            },
            new OcrPhraseDto
            {
                Text = "How are you?",
                X = 90,
                Y = 220,
                Width = 220,
                Height = 60
            }
        };

        return Task.FromResult(phrases);
    }
}