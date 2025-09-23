using System.Text.Json.Serialization;

namespace DiscordWebhookSender.Core.DTO;

/// <summary>
/// Represents the response from Discord when a rate limit is encountered.
/// Contains information about the rate limit, including the message, retry time, and whether it's a global limit.
/// </summary>
public class DiscordRateLimitResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("retry_after")]
    public double RetryAfterSeconds { get; set; }
    
    [JsonPropertyName("global")]
    public bool IsGlobal { get; set; }
}