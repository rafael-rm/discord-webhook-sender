namespace DiscordWebhookSender.Core.Models;

/// <summary>
/// Represents a Discord webhook message that can be sent to a Discord channel.
/// </summary>
public class DiscordWebhookMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordWebhookMessage"/> class.
    /// This constructor is internal to ensure that messages are created through the <see cref="DiscordWebhookMessageBuilder"/> or other methods.
    /// </summary>
    internal DiscordWebhookMessage() { }

    /// <summary>
    /// Gets or sets the content of the message.
    /// This is the main text that will be displayed in the Discord channel.
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Gets or sets the username that will override the webhook's default username.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets whether the message should be sent as text-to-speech (TTS).
    /// When true, Discord will read the message aloud to users with TTS enabled.
    /// </summary>
    public bool? Tts { get; set; }

    /// <summary>
    /// Gets or sets the list of embeds to include in the message.
    /// Embeds provide rich formatting and additional information display.
    /// </summary>
    public List<DiscordEmbed>? Embeds { get; set; }

    /// <summary>
    /// Gets or sets the URL of the avatar image that will override the webhook's default avatar.
    /// </summary>
    public string? AvatarUrl { get; set; }
}