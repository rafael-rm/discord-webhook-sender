using System.Text.Json.Serialization;

namespace DiscordWebhookSender.Models;

public class DiscordEmbedAuthor
{
    public string? Name { get; set; }
    public string? Url { get; set; }
    
    [JsonPropertyName("icon_url")]
    public string? IconUrl { get; set; }
}