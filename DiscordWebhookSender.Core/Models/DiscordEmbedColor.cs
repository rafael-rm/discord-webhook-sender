namespace DiscordWebhookSender.Core.Models;

/// <summary>
/// Predefined colors for Discord embeds.
/// These colors can be used with the DiscordEmbedBuilder.WithColor() method.
/// </summary>
public enum DiscordEmbedColor
{
    /// <summary>
    /// Default color (black).
    /// </summary>
    Default = 0x000000,
    
    /// <summary>
    /// White color.
    /// </summary>
    White = 0xFFFFFF,
    
    /// <summary>
    /// Red color.
    /// </summary>
    Red = 0xFF0000,
    
    /// <summary>
    /// Green color.
    /// </summary>
    Green = 0x00FF00,
    
    /// <summary>
    /// Blue color.
    /// </summary>
    Blue = 0x0000FF,
    
    /// <summary>
    /// Yellow color.
    /// </summary>
    Yellow = 0xFFFF00,
    
    /// <summary>
    /// Orange color.
    /// </summary>
    Orange = 0xFFA500,
    
    /// <summary>
    /// Purple color.
    /// </summary>
    Purple = 0x800080,
    
    /// <summary>
    /// Cyan color.
    /// </summary>
    Cyan = 0x00FFFF,
    
    /// <summary>
    /// Magenta color.
    /// </summary>
    Magenta = 0xFF00FF,
    
    /// <summary>
    /// Teal color.
    /// </summary>
    Teal = 0x008080,
    
    /// <summary>
    /// Gold color.
    /// </summary>
    Gold = 0xFFD700,
    
    /// <summary>
    /// Dark red color.
    /// </summary>
    DarkRed = 0x8B0000,
    
    /// <summary>
    /// Dark green color.
    /// </summary>
    DarkGreen = 0x006400,
    
    /// <summary>
    /// Dark blue color.
    /// </summary>
    DarkBlue = 0x00008B,
    
    /// <summary>
    /// Navy color (dark classic blue).
    /// </summary>
    Navy = 0x000080,
    
    /// <summary>
    /// Olive color (mix between green and yellow, good for intermediate statuses).
    /// </summary>
    Olive = 0x808000,
    
    /// <summary>
    /// Pink color (light tone).
    /// </summary>
    Pink = 0xFFC0CB,
    
    /// <summary>
    /// Dark pink color.
    /// </summary>
    DarkPink = 0xFF1493
}