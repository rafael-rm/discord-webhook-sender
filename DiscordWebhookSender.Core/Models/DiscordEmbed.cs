namespace DiscordWebhookSender.Core.Models;

/// <summary>
/// Represents a Discord embed that can be included in a webhook message.
/// Embeds provide rich formatting and additional information display in Discord messages.
/// </summary>
public class DiscordEmbed
{
    /// <summary>
    /// Gets or sets the title of the embed.
    /// This is displayed prominently at the top of the embed.
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// Gets or sets the description of the embed.
    /// This is the main content text displayed in the embed body.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the URL that the embed title will link to when clicked.
    /// </summary>
    public string? Url { get; set; }
    
    /// <summary>
    /// Gets or sets the timestamp to display in the embed footer.
    /// </summary>
    public DateTimeOffset? Timestamp { get; set; }
    
    /// <summary>
    /// Gets or sets the color of the embed's left border.
    /// This should be an integer representing a hex color value (e.g., 0xFF0000 for red).
    /// </summary>
    public int? Color { get; set; }
    
    /// <summary>
    /// Gets or sets the footer information for the embed.
    /// </summary>
    public DiscordEmbedFooter? Footer { get; set; }
    
    /// <summary>
    /// Gets or sets the main image to display in the embed.
    /// </summary>
    public DiscordEmbedMedia? Image { get; set; }
    
    /// <summary>
    /// Gets or sets the thumbnail image to display in the embed.
    /// </summary>
    public DiscordEmbedMedia? Thumbnail { get; set; }
    
    /// <summary>
    /// Gets or sets the author information for the embed.
    /// </summary>
    public DiscordEmbedAuthor? Author { get; set; }
    
    /// <summary>
    /// Gets or sets the list of fields to display in the embed.
    /// Fields can be used to display structured information in key-value pairs.
    /// </summary>
    public List<DiscordEmbedField>? Fields { get; set; }
}