using DiscordWebhookSender.Models;

namespace DiscordWebhookSender;

public class DiscordEmbedBuilder
{
    private readonly DiscordEmbed _embed = new();

    public DiscordEmbedBuilder WithTitle(string title)
    {
        _embed.Title = title;
        return this;
    }

    public DiscordEmbedBuilder WithDescription(string description)
    {
        _embed.Description = description;
        return this;
    }

    public DiscordEmbedBuilder WithUrl(string url)
    {
        _embed.Url = url;
        return this;
    }

    public DiscordEmbedBuilder WithTimestamp(DateTimeOffset timestamp)
    {
        _embed.Timestamp = timestamp;
        return this;
    }

    public DiscordEmbedBuilder WithColor(int color)
    {
        _embed.Color = color;
        return this;
    }
    
    public DiscordEmbedBuilder WithColor(DiscordEmbedColor color)
    {
        _embed.Color = (int)color;
        return this;
    }
    
    public DiscordEmbedBuilder WithColor(string hexColor)
    {
        if (string.IsNullOrWhiteSpace(hexColor))
            throw new ArgumentException("Hex color cannot be null or empty.", nameof(hexColor));

        var hex = hexColor.TrimStart('#');

        if (!int.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out var colorInt))
            throw new ArgumentException("Invalid hex color format.", nameof(hexColor));

        _embed.Color = colorInt;
        return this;
    }

    public DiscordEmbedBuilder WithFooter(string text, string? iconUrl = null)
    {
        _embed.Footer = new DiscordEmbedFooter
        {
            Text = text,
            IconUrl = iconUrl
        };
        return this;
    }

    public DiscordEmbedBuilder WithImage(string url)
    {
        _embed.Image = new DiscordEmbedMedia { Url = url };
        return this;
    }

    public DiscordEmbedBuilder WithThumbnail(string url)
    {
        _embed.Thumbnail = new DiscordEmbedMedia { Url = url };
        return this;
    }

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

    public DiscordEmbed Build()
    {
        return _embed;
    }
}