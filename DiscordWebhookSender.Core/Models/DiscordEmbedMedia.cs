namespace DiscordWebhookSender.Core.Models;

/// <summary>
/// Represents media content (images) for Discord embeds.
/// Used for both main images and thumbnails in embeds.
/// </summary>
public class DiscordEmbedMedia
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordEmbedMedia"/> class.
    /// This constructor is internal to ensure that media content is created through the builder or other methods.
    /// </summary>
    internal DiscordEmbedMedia() { }
    
    /// <summary>
    /// Gets or sets the URL of the media content.
    /// This should be a direct link to an image file (PNG, JPG, GIF, etc.).
    /// </summary>
    public string? Url { get; set; }
}