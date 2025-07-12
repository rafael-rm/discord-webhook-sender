using DiscordWebhookSender.Models;

namespace DiscordWebhookSender.Interfaces;

public interface IDiscordWebhookClient
{
    Task SendAsync(string webhookUrl, DiscordWebhookMessage message, CancellationToken cancellationToken = default);
}
