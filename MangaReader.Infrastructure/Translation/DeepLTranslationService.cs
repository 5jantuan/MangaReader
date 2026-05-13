using System.Net.Http.Json;
using MangaReader.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MangaReader.Infrastructure.Translation;

public class DeepLTranslationService : ITranslationService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public DeepLTranslationService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> TranslateAsync(
        string text,
        string sourceLanguageCode,
        string targetLanguageCode)
    {
        var apiKey = _configuration["DeepL:ApiKey"];
        var baseUrl = _configuration["DeepL:BaseUrl"];

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("DeepL API key is missing.");

        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new InvalidOperationException("DeepL base URL is missing.");

        var sourceLang = NormalizeLanguageCode(sourceLanguageCode);
        var targetLang = NormalizeLanguageCode(targetLanguageCode);

        using var request = new HttpRequestMessage(HttpMethod.Post, baseUrl);

        request.Headers.Add("Authorization", $"DeepL-Auth-Key {apiKey}");

        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["text"] = text,
            ["source_lang"] = sourceLang,
            ["target_lang"] = targetLang
        });

        var response = await _httpClient.SendAsync(request);

        var responseText = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"DeepL error {(int)response.StatusCode}: {responseText}");

        var result = await response.Content.ReadFromJsonAsync<DeepLResponse>();

        return result?.Translations?.FirstOrDefault()?.Text
            ?? throw new InvalidOperationException("DeepL returned empty translation.");
    }

    private static string NormalizeLanguageCode(string code)
    {
        return code.ToLower() switch
        {
            "en" => "EN",
            "ru" => "RU",
            "ja" => "JA",
            _ => code.ToUpper()
        };
    }

    private class DeepLResponse
    {
        public List<DeepLTranslationItem> Translations { get; set; } = new();
    }

    private class DeepLTranslationItem
    {
        public string Text { get; set; } = string.Empty;
    }
}