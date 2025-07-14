using System.Text.Json.Serialization;

namespace DiscordWebhookSender.Core.Models;

/// <summary>
/// Represents the footer information for a Discord embed.
/// </summary>
public class DiscordEmbedFooter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordEmbedFooter"/> class.
    /// This constructor is internal to ensure that footer information is created through the builder or other methods.
    /// </summary>
    [JsonConstructor]
    internal DiscordEmbedFooter() { }
    
    /// <summary>
    /// Gets or sets the footer text.
    /// This is displayed at the bottom of the embed.
    /// </summary>
    public string? Text { get; set; }
    
    /// <summary>
    /// Gets or sets the URL of the footer icon.
    /// This is displayed as a small icon next to the footer text.
    /// </summary>
    public string? IconUrl { get; set; }
}