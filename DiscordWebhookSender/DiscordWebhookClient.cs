using System.Text;
using System.Text.Json;
using DiscordWebhookSender.Interfaces;
using DiscordWebhookSender.Models;

namespace DiscordWebhookSender;

/// <summary>
/// Client for sending Discord webhook messages.
/// Provides functionality to send messages and embeds to Discord channels via webhooks.
/// Implements the Singleton pattern to ensure only one instance exists.
/// </summary>
public class DiscordWebhookClient : IDiscordWebhookClient
{
    private static DiscordWebhookClient? _instance;
    private static readonly object _lock = new object();
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Private constructor to prevent direct instantiation.
    /// Use the Get() method to obtain the singleton instance.
    /// </summary>
    /// <param name="httpClient">Optional HttpClient instance. If not provided, a new instance will be created.</param>
    private DiscordWebhookClient(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }

    /// <summary>
    /// Gets the singleton instance of DiscordWebhookClient.
    /// Creates a new instance if one doesn't exist.
    /// </summary>
    /// <param name="httpClient">Optional HttpClient instance. Only used when creating a new instance.</param>
    /// <returns>The singleton instance of DiscordWebhookClient.</returns>
    public static DiscordWebhookClient Get(HttpClient? httpClient = null)
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new DiscordWebhookClient(httpClient);
                }
            }
        }
        return _instance;
    }

    /// <summary>
    /// Sends a Discord webhook message asynchronously.
    /// </summary>
    /// <param name="webhookUrl">The Discord webhook URL to send the message to.</param>
    /// <param name="message">The Discord webhook message to send.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    /// <exception cref="ArgumentException">Thrown when webhookUrl is null, empty, or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when message is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails or returns an error status code.</exception>
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

    /// <summary>
    /// Sends a simple text message to Discord webhook asynchronously.
    /// </summary>
    /// <param name="webhookUrl">The Discord webhook URL to send the message to.</param>
    /// <param name="content">The text content to send.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    /// <exception cref="ArgumentException">Thrown when webhookUrl is null, empty, or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when content is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails or returns an error status code.</exception>
    public async Task SendAsync(string webhookUrl, string content, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(content);
        var message = new DiscordWebhookMessage { Content = content };
        
        await SendAsync(webhookUrl, message, cancellationToken);
    }

    /// <summary>
    /// Sends a Discord embed to webhook asynchronously.
    /// </summary>
    /// <param name="webhookUrl">The Discord webhook URL to send the message to.</param>
    /// <param name="embed">The Discord embed to send.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    /// <exception cref="ArgumentException">Thrown when webhookUrl is null, empty, or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when embed is null.</exception>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails or returns an error status code.</exception>
    public async Task SendAsync(string webhookUrl, DiscordEmbed embed, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(embed);
        var message = new DiscordWebhookMessage { Embeds = [embed] };
        
        await SendAsync(webhookUrl, message, cancellationToken);
    }
}