using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using DiscordWebhookSender.Core.DTO;
using DiscordWebhookSender.Core.Exceptions;
using DiscordWebhookSender.Core.Interfaces;
using DiscordWebhookSender.Core.Models;
using DiscordWebhookSender.Core.Validation;

namespace DiscordWebhookSender.Core;

/// <summary>
/// Client for sending Discord webhook messages.
/// Provides functionality to send messages and embeds to Discord channels via webhooks.
/// Implements the Singleton pattern to ensure only one instance exists.
/// </summary>
public class DiscordWebhookClient : IDiscordWebhookClient
{
    private static DiscordWebhookClient? _instance;
    private static readonly object Lock = new();
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly bool _enableRateLimitHandling; 
    private readonly int _maxRetries;

    /// <summary>
    /// Private constructor to prevent direct instantiation.
    /// Use the Get() method to obtain the singleton instance.
    /// </summary>
    /// <param name="httpClient">HttpClient instance. If not provided, a new instance will be created.</param>
    /// <param name="enableRateLimitHandling">Indicates whether to enable rate limit handling. </param>
    /// <param name="maxRetries">The maximum number of retries for sending a webhook in case of transient failures.</param>
    private DiscordWebhookClient(HttpClient? httpClient, bool enableRateLimitHandling, int maxRetries)
    {
        _httpClient = httpClient ?? new HttpClient();
        _enableRateLimitHandling = enableRateLimitHandling;
        _maxRetries = maxRetries;
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
    /// <param name="enableRateLimitHandling">Indicates whether to enable rate limit handling. Only used when creating a new instance.</param>
    /// <param name="maxRetries">The maximum number of retries for sending a webhook in case of transient failures. Only used when creating a new instance.</param>
    /// <returns>The singleton instance of DiscordWebhookClient.</returns>
    public static DiscordWebhookClient Get(HttpClient? httpClient = null, bool enableRateLimitHandling = true, int maxRetries = 3)
    {
        lock (Lock)
        {
            if (_instance != null) return _instance;
            
            _instance = new DiscordWebhookClient(httpClient, enableRateLimitHandling, maxRetries);
            
            return _instance;
        }
    }

    /// <summary>
    /// Resets the singleton instance. This method is primarily for testing purposes.
    /// </summary>
    public static void Reset()
    {
        lock (Lock)
        {
            _instance?._httpClient.Dispose();
            _instance = null;
        }
    }

    /// <summary>
    /// Sends a Discord webhook message asynchronously.
    /// <para>
    /// Validates the webhook URL and message, then forwards the request to <see cref="ProcessWebhookAsync"/>.
    /// Respects the rate limit handling configuration set in <see cref="DiscordWebhookClient"/>.
    /// </para>
    /// </summary>
    /// <param name="webhookUrl">The Discord webhook URL to send the message to.</param>
    /// <param name="message">The <see cref="DiscordWebhookMessage"/> payload to send.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    /// <exception cref="DiscordValidationException">Thrown if the webhook URL or message fails validation.</exception>
    /// <exception cref="HttpRequestException">Thrown if the HTTP request fails or returns an error status code.</exception>
    public async Task SendAsync(string webhookUrl, DiscordWebhookMessage message, CancellationToken cancellationToken = default)
    {
        await ProcessWebhookAsync(webhookUrl, message, cancellationToken);
    }

    /// <summary>
    /// Sends a simple text message to a Discord webhook asynchronously.
    /// <para>
    /// Wraps the text content into a <see cref="DiscordWebhookMessage"/> and forwards to <see cref="ProcessWebhookAsync"/>.
    /// Respects the rate limit handling configuration set in <see cref="DiscordWebhookClient"/>.
    /// </para>
    /// </summary>
    /// <param name="webhookUrl">The Discord webhook URL to send the message to.</param>
    /// <param name="content">The text content to send (max 2000 characters).</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    /// <exception cref="DiscordValidationException">Thrown if the content exceeds 2000 characters.</exception>
    /// <exception cref="HttpRequestException">Thrown if the HTTP request fails or returns an error status code.</exception>
    public async Task SendAsync(string webhookUrl, string content, CancellationToken cancellationToken = default)
    {
        var message = new DiscordWebhookMessage { Content = content };
        
        await ProcessWebhookAsync(webhookUrl, message, cancellationToken);
    }

    /// <summary>
    /// Sends a Discord embed to a webhook asynchronously.
    /// <para>
    /// Wraps the embed into a <see cref="DiscordWebhookMessage"/> and forwards to <see cref="ProcessWebhookAsync"/>.
    /// Respects the rate limit handling configuration set in <see cref="DiscordWebhookClient"/>.
    /// </para>
    /// </summary>
    /// <param name="webhookUrl">The Discord webhook URL to send the message to.</param>
    /// <param name="embed">The <see cref="DiscordEmbed"/> to send.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    /// <exception cref="DiscordValidationException">Thrown if the embed fails validation.</exception>
    /// <exception cref="HttpRequestException">Thrown if the HTTP request fails or returns an error status code.</exception>
    public async Task SendAsync(string webhookUrl, DiscordEmbed embed, CancellationToken cancellationToken = default)
    {
        var message = new DiscordWebhookMessage { Embeds = [embed] };
        
        await ProcessWebhookAsync(webhookUrl, message, cancellationToken);
    }
    
    /// <summary>
    /// Executes the HTTP POST request to send a Discord webhook message.
    /// <para>
    /// Validates the webhook URL and message before sending. Handles rate limiting
    /// (HTTP 429) automatically if <see cref="_enableRateLimitHandling"/> is enabled.
    /// Retries the request up to <see cref="_maxRetries"/> times in case of transient failures.
    /// </para>
    /// </summary>
    /// <param name="webhookUrl">The Discord webhook URL to send the message to.</param>
    /// <param name="message">The Discord webhook message payload.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <exception cref="DiscordValidationException">Thrown when the webhook URL or message fails validation.</exception>
    /// <exception cref="HttpRequestException">
    /// Thrown when the HTTP request fails, returns an error status code, or
    /// exceeds the maximum number of retries.
    /// </exception>
    private async Task ProcessWebhookAsync(string webhookUrl, DiscordWebhookMessage message, CancellationToken cancellationToken = default)
    {
        DiscordValidator.ValidateWebhookUrl(webhookUrl);
        DiscordValidator.ValidateWebhookMessage(message);

        var payload = JsonSerializer.Serialize(message, _jsonOptions);
        
        for (var i = 0; i < _maxRetries; i++)
        {
            using var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(webhookUrl, content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return;
            }

            if (response.StatusCode == HttpStatusCode.TooManyRequests && _enableRateLimitHandling)
            {
                await HandleRateLimitAsync(response, cancellationToken); // TODO: Global rate limit handling strategy for parallel requests
                continue;
            }

            if (response.StatusCode == HttpStatusCode.TooManyRequests && !_enableRateLimitHandling)
            {
                throw new HttpRequestException("Rate limit exceeded (HTTP 429). Enable rate limit handling to automatically manage retries.", null, response.StatusCode);
            }
            
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Failed to send Discord webhook. Status: {(int)response.StatusCode}. Body: {errorBody}", null, response.StatusCode);
        }
        
        throw new HttpRequestException($"Failed to send Discord webhook after {_maxRetries} attempts due to persistent errors.", null);
    }
    
    /// <summary>
    /// Handles Discord rate limiting by delaying the request based on the Retry-After value
    /// returned in the HTTP 429 response.
    /// <para>
    /// This method reads the <see cref="DiscordRateLimitResponse"/> from the response content
    /// and pauses the execution for the duration specified in <c>RetryAfterSeconds</c>.
    /// A small buffer (100 milliseconds) is added to ensure compliance with Discord's rate limits.
    /// </para>
    /// </summary>
    /// <param name="response">The HTTP response containing the rate limit information (HTTP 429).</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the delay.</param>
    private async Task HandleRateLimitAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var rateLimitResponse = await response.Content.ReadFromJsonAsync<DiscordRateLimitResponse>(cancellationToken: cancellationToken);

        if (rateLimitResponse?.RetryAfterSeconds > 0)
        {
            var delay = TimeSpan.FromSeconds(rateLimitResponse.RetryAfterSeconds).Add(TimeSpan.FromMilliseconds(100));
            await Task.Delay(delay, cancellationToken);
        }
    }
}