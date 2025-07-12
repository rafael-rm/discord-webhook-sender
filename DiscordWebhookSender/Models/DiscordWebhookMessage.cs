using System.Text.Json.Serialization;

namespace DiscordWebhookSender.Models;

public class DiscordWebhookMessage
{
    public string? Content { get; set; }
    public string? Username { get; set; }
    public bool? Tts { get; set; }
    public List<DiscordEmbed>? Embeds { get; set; }
    
    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; set; }
}