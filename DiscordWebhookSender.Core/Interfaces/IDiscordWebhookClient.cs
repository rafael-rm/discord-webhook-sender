using DiscordWebhookSender.Core.Models;

namespace DiscordWebhookSender.Core.Interfaces;

/// <summary>
/// Interface for Discord webhook client implementations.
/// Defines the contract for sending Discord webhook messages.
/// </summary>
public interface IDiscordWebhookClient
{
    /// <summary>
    /// Sends a Discord webhook message asynchronously.
    /// </summary>
    /// <param name="webhookUrl">The Discord webhook URL to send the message to.</param>
    /// <param name="message">The Discord webhook message to send.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    Task SendAsync(string webhookUrl, DiscordWebhookMessage message, CancellationToken cancellationToken = default);
    
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
    Task SendAsync(string webhookUrl, string content, CancellationToken cancellationToken = default);
    
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
    Task SendAsync(string webhookUrl, DiscordEmbed embed, CancellationToken cancellationToken = default);
}
