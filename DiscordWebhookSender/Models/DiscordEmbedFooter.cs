using System.Text.Json.Serialization;

namespace DiscordWebhookSender.Models;

public class DiscordEmbedFooter
{
    public string? Text { get; set; }
    
    [JsonPropertyName("icon_url")]
    public string? IconUrl { get; set; }
}