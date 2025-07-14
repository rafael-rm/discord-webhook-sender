using DiscordWebhookSender.Core.Exceptions;
using DiscordWebhookSender.Core.Models;
using DiscordWebhookSender.Core.Validation;

namespace DiscordWebhookSender.Core;

/// <summary>
/// Fluent builder for creating Discord webhook messages.
/// Provides a chainable API for building Discord webhook messages with content, embeds, and customization options.
/// </summary>
public class DiscordWebhookMessageBuilder
{
    private readonly DiscordWebhookMessage _message = new();

    /// <summary>
    /// Sets the content of the webhook message.
    /// </summary>
    /// <param name="content">The main text content of the message.</param>
    /// <returns>The current DiscordWebhookMessageBuilder instance for method chaining.</returns>
    public DiscordWebhookMessageBuilder WithContent(string content)
    {
        _message.Content = content;
        return this;
    }

    /// <summary>
    /// Sets the username that will override the webhook's default username.
    /// </summary>
    /// <param name="username">The custom username to display for this message.</param>
    /// <returns>The current DiscordWebhookMessageBuilder instance for method chaining.</returns>
    public DiscordWebhookMessageBuilder WithUsername(string username)
    {
        _message.Username = username;
        return this;
    }

    /// <summary>
    /// Sets whether the message should be sent as text-to-speech (TTS).
    /// </summary>
    /// <param name="tts">True to enable text-to-speech, false to disable.</param>
    /// <returns>The current DiscordWebhookMessageBuilder instance for method chaining.</returns>
    public DiscordWebhookMessageBuilder WithTts(bool tts)
    {
        _message.Tts = tts;
        return this;
    }

    /// <summary>
    /// Sets the URL of the avatar image that will override the webhook's default avatar.
    /// </summary>
    /// <param name="avatarUrl">The URL of the avatar image.</param>
    /// <returns>The current DiscordWebhookMessageBuilder instance for method chaining.</returns>
    public DiscordWebhookMessageBuilder WithAvatarUrl(string avatarUrl)
    {
        _message.AvatarUrl = avatarUrl;
        return this;
    }

    /// <summary>
    /// Adds a single embed to the message.
    /// </summary>
    /// <param name="embed">The DiscordEmbed to add to the message.</param>
    /// <returns>The current DiscordWebhookMessageBuilder instance for method chaining.</returns>
    public DiscordWebhookMessageBuilder AddEmbed(DiscordEmbed embed)
    {
        _message.Embeds ??= [];
        _message.Embeds.Add(embed);
        return this;
    }

    /// <summary>
    /// Adds multiple embeds to the message.
    /// </summary>
    /// <param name="embeds">The collection of DiscordEmbed objects to add to the message.</param>
    /// <returns>The current DiscordWebhookMessageBuilder instance for method chaining.</returns>
    public DiscordWebhookMessageBuilder AddEmbed(IEnumerable<DiscordEmbed> embeds)
    {
        _message.Embeds ??= [];
        _message.Embeds.AddRange(embeds);
        return this;
    }

    /// <summary>
    /// Adds multiple embeds to the message using params.
    /// </summary>
    /// <param name="embeds">The DiscordEmbed objects to add to the message.</param>
    /// <returns>The current DiscordWebhookMessageBuilder instance for method chaining.</returns>
    public DiscordWebhookMessageBuilder AddEmbed(params DiscordEmbed[] embeds)
    {
        return AddEmbed(embeds.AsEnumerable());
    }

    /// <summary>
    /// Clears all embeds from the message.
    /// </summary>
    /// <returns>The current DiscordWebhookMessageBuilder instance for method chaining.</returns>
    public DiscordWebhookMessageBuilder ClearEmbeds()
    {
        _message.Embeds = null;
        return this;
    }

    /// <summary>
    /// Builds and returns the final DiscordWebhookMessage instance.
    /// </summary>
    /// <returns>A DiscordWebhookMessage instance with all the configured properties.</returns>
    /// <exception cref="DiscordValidationException">Thrown when the message exceeds Discord's limits or contains invalid data.</exception>
    public DiscordWebhookMessage Build()
    {
        DiscordValidator.ValidateWebhookMessage(_message);
        return _message;
    }
} 