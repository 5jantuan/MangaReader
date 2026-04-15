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
                Height = 50,
                Confidence = 0.95m
            },
            new OcrPhraseDto
            {
                Text = "How are you?",
                X = 90,
                Y = 220,
                Width = 220,
                Height = 60,
                Confidence = 0.88m
            },
            new OcrPhraseDto
            {
                Text = ".",
                X = 30,
                Y = 40,
                Width = 10,
                Height = 10,
                Confidence = 0.15m
            }
        };

        return Task.FromResult(phrases);
    }
}