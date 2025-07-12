using System.Text;
using System.Text.Json;
using DiscordWebhookSender.Interfaces;
using DiscordWebhookSender.Models;

namespace DiscordWebhookSender;

public class DiscordWebhookClient : IDiscordWebhookClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public DiscordWebhookClient(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }

    public async Task SendAsync(string webhookUrl, DiscordWebhookMessage message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(webhookUrl))
            throw new ArgumentException("Webhook URL cannot be null or empty.", nameof(webhookUrl));

        ArgumentNullException.ThrowIfNull(message);

        var payload = JsonSerializer.Serialize(message, _jsonOptions);
        using var content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(webhookUrl, content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Failed to send Discord webhook. Status: {(int)response.StatusCode}. Body: {errorBody}");
        }
    }
}