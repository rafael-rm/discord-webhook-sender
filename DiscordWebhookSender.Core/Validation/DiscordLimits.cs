namespace DiscordWebhookSender.Core.Validation;

/// <summary>
/// Contains Discord API limits and constraints for webhook messages and embeds.
/// These limits are enforced by Discord and must be respected to avoid API errors.
/// </summary>
public static class DiscordLimits
{
    /// <summary>
    /// Maximum length for message content (2000 characters).
    /// </summary>
    public const int MaxContentLength = 2000;
    
    /// <summary>
    /// Maximum length for embed title (256 characters).
    /// </summary>
    public const int MaxTitleLength = 256;
    
    /// <summary>
    /// Maximum length for embed description (4096 characters).
    /// </summary>
    public const int MaxDescriptionLength = 4096;
    
    /// <summary>
    /// Maximum length for field name (256 characters).
    /// </summary>
    public const int MaxFieldNameLength = 256;
    
    /// <summary>
    /// Maximum length for field value (1024 characters).
    /// </summary>
    public const int MaxFieldValueLength = 1024;
    
    /// <summary>
    /// Maximum length for footer text (2048 characters).
    /// </summary>
    public const int MaxFooterTextLength = 2048;
    
    /// <summary>
    /// Maximum length for author name (256 characters).
    /// </summary>
    public const int MaxAuthorNameLength = 256;
    
    /// <summary>
    /// Maximum length for username (80 characters).
    /// </summary>
    public const int MaxUsernameLength = 80;
    
    /// <summary>
    /// Maximum number of fields per embed (25 fields).
    /// </summary>
    public const int MaxFieldsPerEmbed = 25;
    
    /// <summary>
    /// Maximum number of embeds per message (10 embeds).
    /// </summary>
    public const int MaxEmbedsPerMessage = 10;
    
    /// <summary>
    /// Maximum total length of all embed content (6000 characters).
    /// This includes title, description, field names, field values, footer text, and author name.
    /// </summary>
    public const int MaxTotalEmbedLength = 6000;
} 