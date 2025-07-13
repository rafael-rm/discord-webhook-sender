using DiscordWebhookSender.Core.Exceptions;
using DiscordWebhookSender.Core.Models;
using DiscordWebhookSender.Core.Validation;

namespace DiscordWebhookSender.Core;

/// <summary>
/// Fluent builder for creating Discord embeds.
/// Provides a chainable API for building rich Discord embed messages.
/// </summary>
public class DiscordEmbedBuilder
{
    private readonly DiscordEmbed _embed = new();

    /// <summary>
    /// Sets the title of the embed.
    /// </summary>
    /// <param name="title">The title text to display in the embed.</param>
    /// <returns>The current DiscordEmbedBuilder instance for method chaining.</returns>
    public DiscordEmbedBuilder WithTitle(string title)
    {
        _embed.Title = title;
        return this;
    }

    /// <summary>
    /// Sets the description of the embed.
    /// </summary>
    /// <param name="description">The description text to display in the embed.</param>
    /// <returns>The current DiscordEmbedBuilder instance for method chaining.</returns>
    public DiscordEmbedBuilder WithDescription(string description)
    {
        _embed.Description = description;
        return this;
    }

    /// <summary>
    /// Sets the URL that the embed title will link to.
    /// </summary>
    /// <param name="url">The URL to make the title clickable.</param>
    /// <returns>The current DiscordEmbedBuilder instance for method chaining.</returns>
    public DiscordEmbedBuilder WithUrl(string url)
    {
        _embed.Url = url;
        return this;
    }

    /// <summary>
    /// Sets the timestamp of the embed.
    /// </summary>
    /// <param name="timestamp">The timestamp to display in the embed footer.</param>
    /// <returns>The current DiscordEmbedBuilder instance for method chaining.</returns>
    public DiscordEmbedBuilder WithTimestamp(DateTimeOffset timestamp)
    {
        _embed.Timestamp = timestamp;
        return this;
    }
    
    /// <summary>
    /// Sets the timestamp of the embed using a DateTime value.
    /// /// </summary>
    /// <param name="dateTime">The DateTime to convert to a DateTimeOffset for the embed timestamp.</param>
    /// <returns>The current DiscordEmbedBuilder instance for method chaining.</returns>
    public DiscordEmbedBuilder WithTimestamp(DateTime dateTime)
    {
        _embed.Timestamp = new DateTimeOffset(dateTime);
        return this;
    }

    /// <summary>
    /// Sets the color of the embed using an integer value.
    /// </summary>
    /// <param name="color">The color value as an integer (e.g., 0xFF0000 for red).</param>
    /// <returns>The current DiscordEmbedBuilder instance for method chaining.</returns>
    public DiscordEmbedBuilder WithColor(int color)
    {
        _embed.Color = color;
        return this;
    }
    
    /// <summary>
    /// Sets the color of the embed using a predefined color from DiscordEmbedColor enum.
    /// </summary>
    /// <param name="color">The predefined color from DiscordEmbedColor enum.</param>
    /// <returns>The current DiscordEmbedBuilder instance for method chaining.</returns>
    public DiscordEmbedBuilder WithColor(DiscordEmbedColor color)
    {
        _embed.Color = (int)color;
        return this;
    }
    
    /// <summary>
    /// Sets the color of the embed using a hex color string.
    /// </summary>
    /// <param name="hexColor">The hex color string (e.g., "#FF0000" or "FF0000").</param>
    /// <returns>The current DiscordEmbedBuilder instance for method chaining.</returns>
    /// <exception cref="DiscordValidationException">Thrown when hexColor is null, empty, or in invalid format.</exception>
    public DiscordEmbedBuilder WithColor(string hexColor)
    {
        DiscordValidator.ValidateHexColor(hexColor);
        
        var hex = hexColor.TrimStart('#');
        var colorInt = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        _embed.Color = colorInt;
        return this;
    }

    /// <summary>
    /// Sets the footer of the embed.
    /// </summary>
    /// <param name="text">The footer text to display.</param>
    /// <param name="iconUrl">Optional URL for the footer icon.</param>
    /// <returns>The current DiscordEmbedBuilder instance for method chaining.</returns>
    public DiscordEmbedBuilder WithFooter(string text, string? iconUrl = null)
    {
        _embed.Footer = new DiscordEmbedFooter
        {
            Text = text,
            IconUrl = iconUrl
        };
        return this;
    }

    /// <summary>
    /// Sets the main image of the embed.
    /// </summary>
    /// <param name="url">The URL of the image to display.</param>
    /// <returns>The current DiscordEmbedBuilder instance for method chaining.</returns>
    public DiscordEmbedBuilder WithImage(string url)
    {
        _embed.Image = new DiscordEmbedMedia { Url = url };
        return this;
    }

    /// <summary>
    /// Sets the thumbnail image of the embed.
    /// </summary>
    /// <param name="url">The URL of the thumbnail image to display.</param>
    /// <returns>The current DiscordEmbedBuilder instance for method chaining.</returns>
    public DiscordEmbedBuilder WithThumbnail(string url)
    {
        _embed.Thumbnail = new DiscordEmbedMedia { Url = url };
        return this;
    }

    /// <summary>
    /// Sets the author information of the embed.
    /// </summary>
    /// <param name="name">The name of the author.</param>
    /// <param name="url">Optional URL that the author name will link to.</param>
    /// <param name="iconUrl">Optional URL for the author's avatar icon.</param>
    /// <returns>The current DiscordEmbedBuilder instance for method chaining.</returns>
    public DiscordEmbedBuilder WithAuthor(string name, string? url = null, string? iconUrl = null)
    {
        _embed.Author = new DiscordEmbedAuthor
        {
            Name = name,
            Url = url,
            IconUrl = iconUrl
        };
        return this;
    }

    /// <summary>
    /// Adds a field to the embed.
    /// </summary>
    /// <param name="name">The name/title of the field.</param>
    /// <param name="value">The value/content of the field.</param>
    /// <param name="inline">Whether the field should be displayed inline (side by side with other inline fields).</param>
    /// <returns>The current DiscordEmbedBuilder instance for method chaining.</returns>
    public DiscordEmbedBuilder AddField(string name, string value, bool inline = false)
    {
        _embed.Fields ??= new List<DiscordEmbedField>();
        
        _embed.Fields.Add(new DiscordEmbedField
        {
            Name = name,
            Value = value,
            Inline = inline
        });
        return this;
    }

    /// <summary>
    /// Builds and returns the final DiscordEmbed instance.
    /// </summary>
    /// <returns>A DiscordEmbed instance with all the configured properties.</returns>
    /// <exception cref="DiscordValidationException">Thrown when the embed exceeds Discord's total content limits.</exception>
    public DiscordEmbed Build()
    {
        DiscordValidator.ValidateEmbed(_embed);
        return _embed;
    }
}