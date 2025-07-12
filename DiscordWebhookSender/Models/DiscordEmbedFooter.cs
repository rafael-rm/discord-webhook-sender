using System.Text.Json.Serialization;

namespace DiscordWebhookSender.Models;

/// <summary>
/// Represents the footer information for a Discord embed.
/// </summary>
public class DiscordEmbedFooter
{
    /// <summary>
    /// Gets or sets the footer text.
    /// This is displayed at the bottom of the embed.
    /// </summary>
    public string? Text { get; set; }
    
    /// <summary>
    /// Gets or sets the URL of the footer icon.
    /// This is displayed as a small icon next to the footer text.
    /// </summary>
    [JsonPropertyName("icon_url")]
    public string? IconUrl { get; set; }
}