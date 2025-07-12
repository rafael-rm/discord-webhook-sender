using DiscordWebhookSender.Models;

namespace DiscordWebhookSender.Interfaces;

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
}
