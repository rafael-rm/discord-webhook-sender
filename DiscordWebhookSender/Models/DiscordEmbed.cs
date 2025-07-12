namespace DiscordWebhookSender.Models;

public class DiscordEmbed
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public int? Color { get; set; }
    public DiscordEmbedFooter? Footer { get; set; }
    public DiscordEmbedMedia? Image { get; set; }
    public DiscordEmbedMedia? Thumbnail { get; set; }
    public DiscordEmbedAuthor? Author { get; set; }
    public List<DiscordEmbedField>? Fields { get; set; }
}