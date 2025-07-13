namespace DiscordWebhookSender.Core.Models;

/// <summary>
/// Represents a field within a Discord embed.
/// Fields are used to display structured information in key-value pairs.
/// </summary>
public class DiscordEmbedField
{
    /// <summary>
    /// Gets or sets the name/title of the field.
    /// This is displayed as the field's header.
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Gets or sets the value/content of the field.
    /// This is displayed as the field's content below the name.
    /// </summary>
    public string? Value { get; set; }
    
    /// <summary>
    /// Gets or sets whether the field should be displayed inline.
    /// Inline fields are displayed side by side with other inline fields.
    /// Non-inline fields take up the full width of the embed.
    /// </summary>
    public bool Inline { get; set; }
}