using System.Net.Http.Headers;
using System.Net.Http.Json;
using MangaReader.Application.DTOs;
using MangaReader.Application.Interfaces;
using Microsoft.Extensions.Hosting;

namespace MangaReader.Infrastructure.Ocr;

public class HttpOcrService : IOcrService
{
    private readonly HttpClient _httpClient;
    private readonly IHostEnvironment _environment;

    public HttpOcrService(HttpClient httpClient, IHostEnvironment environment)
    {
        _httpClient = httpClient;
        _environment = environment;
    }

    public async Task<List<OcrPhraseDto>> ExtractPhrasesAsync(string imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            throw new ArgumentException("Image path cannot be empty.", nameof(imagePath));

        var relativePath = imagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_environment.ContentRootPath, "wwwroot", relativePath);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"Image file not found: {fullPath}");

        await using var fileStream = File.OpenRead(fullPath);

        using var form = new MultipartFormDataContent();
        using var fileContent = new StreamContent(fileStream);

        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        form.Add(fileContent, "file", Path.GetFileName(fullPath));

        // TODO: later pass Manga original language here instead of hardcoded "en".
        form.Add(new StringContent("en"), "lang");

        var response = await _httpClient.PostAsync("/ocr", form);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"OCR service returned {(int)response.StatusCode}: {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<List<OcrPhraseDto>>();
        return result ?? new List<OcrPhraseDto>();
    }
}