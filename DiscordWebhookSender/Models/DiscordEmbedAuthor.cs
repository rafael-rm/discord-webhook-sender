using System.Text.Json.Serialization;

namespace DiscordWebhookSender.Models;

/// <summary>
/// Represents the author information for a Discord embed.
/// </summary>
public class DiscordEmbedAuthor
{
    /// <summary>
    /// Gets or sets the name of the author.
    /// This is displayed prominently in the embed.
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Gets or sets the URL that the author name will link to when clicked.
    /// </summary>
    public string? Url { get; set; }
    
    /// <summary>
    /// Gets or sets the URL of the author's avatar icon.
    /// This is displayed as a small icon next to the author name.
    /// </summary>
    public string? IconUrl { get; set; }
}