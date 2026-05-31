using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MuseSpace.BLL.Interfaces.Services;
using System.Net.Http.Json;
using System.Text.Json;

namespace MuseSpace.BLL.Services;

public class GeminiDescriptionService : IAiDescriptionService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GeminiDescriptionService> _logger;

    public GeminiDescriptionService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<GeminiDescriptionService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string?> GenerateDescriptionAsync(string imageUrl, CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration["Gemini:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogWarning("Gemini API key is not configured.");
            return null;
        }

        try
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";

            // Gemini Vision API payload format requires fetching the image or sending a URI it can access.
            // Since it's a public Cloudinary URL, we can use the text prompt + image URL approach, 
            // but native Gemini API often prefers inline base64 if it can't scrape URLs.
            // For a university project mock/simplification, we'll ask it to describe the image from the URL.
            // Note: If Gemini cannot fetch the image directly from the URL in the prompt, 
            // you'd normally download it and send it as inlineData. 
            // For brevity and assuming Gemini can read public URLs in prompts:
            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = $"Please describe this artwork in 2-3 sentences. Focus on the visual style, subject matter, colors, and mood. Image URL: {imageUrl}" }
                        }
                    }
                }
            };

            var response = await _httpClient.PostAsJsonAsync(url, payload, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Gemini API call failed: {Error}", error);
                return null;
            }

            var resultStr = await response.Content.ReadAsStringAsync(cancellationToken);
            using var document = JsonDocument.Parse(resultStr);

            // Navigate Gemini's response structure: candidates[0].content.parts[0].text
            var text = document.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return text;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate AI description for {ImageUrl}", imageUrl);
            return null;
        }
    }
}
