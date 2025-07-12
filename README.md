# Discord Webhook Sender

A .NET library for sending Discord webhooks with rich embeds and message formatting. This library provides a simple and intuitive API for creating and sending Discord webhook messages with support for embeds, colors, fields, and more.

## Features

- **Simple Webhook Client**: Easy-to-use HTTP client for sending Discord webhooks
- **Rich Embed Builder**: Fluent API for creating beautiful Discord embeds
- **Color Support**: Built-in color constants and hex color support
- **Message Customization**: Support for username, avatar, content, and TTS
- **Field Management**: Add inline and block fields to embeds
- **Media Support**: Add images and thumbnails to embeds
- **Author & Footer**: Customize embed author and footer information
- **Timestamp Support**: Add timestamps to embeds
- **Error Handling**: Proper exception handling for failed webhook requests

## Installation

```bash
dotnet add package DiscordWebhookSender
```

## Quick Start

```csharp
using DiscordWebhookSender;
using DiscordWebhookSender.Models;

// Create an embed
var embed = new DiscordEmbedBuilder()
    .WithTitle("🚀 Deploy Completed Successfully!")
    .WithDescription("The new application version has been published and is operating normally.")
    .WithUrl("https://yourcompany.com/deploys/2.1.0")
    .WithTimestamp(DateTimeOffset.UtcNow)
    .WithColor(DiscordEmbedColor.Green)
    .WithAuthor("CI/CD Bot", "https://yourcompany.com", "https://example.com/avatar.png")
    .WithThumbnail("https://example.com/thumbnail.png")
    .AddField("Version", "2.1.0", true)
    .AddField("Environment", "Production", true)
    .AddField("Duration", "12 minutes", true)
    .WithFooter("CI Pipeline • 2025", "https://example.com/footer.png")
    .Build();

// Create the webhook message
var message = new DiscordWebhookMessage
{
    Username = "CI/CD Bot",
    AvatarUrl = "https://example.com/avatar.png",
    Content = "✅ Automatic update completed",
    Embeds = [embed]
};

// Send the webhook
var client = new DiscordWebhookClient();
await client.SendAsync("YOUR_DISCORD_WEBHOOK_URL", message);
```

## API Reference

### DiscordWebhookClient

The main client for sending Discord webhooks.

```csharp
var client = new DiscordWebhookClient();
await client.SendAsync(webhookUrl, message, cancellationToken);
```

### DiscordEmbedBuilder

Fluent API for building Discord embeds.

#### Basic Methods
- `WithTitle(string title)` - Set the embed title
- `WithDescription(string description)` - Set the embed description
- `WithUrl(string url)` - Set the embed URL
- `WithTimestamp(DateTimeOffset timestamp)` - Set the embed timestamp

#### Color Methods
- `WithColor(int color)` - Set color using integer value
- `WithColor(DiscordEmbedColor color)` - Set color using predefined enum
- `WithColor(string hexColor)` - Set color using hex string (e.g., "#FF0000")

#### Media Methods
- `WithImage(string url)` - Add an image to the embed
- `WithThumbnail(string url)` - Add a thumbnail to the embed

#### Author & Footer
- `WithAuthor(string name, string? url, string? iconUrl)` - Set embed author
- `WithFooter(string text, string? iconUrl)` - Set embed footer

#### Fields
- `AddField(string name, string value, bool inline)` - Add a field to the embed

### DiscordEmbedColor Enum

Predefined colors for easy use:
- `Default`, `White`, `Red`, `Green`, `Blue`
- `Yellow`, `Orange`, `Purple`, `Cyan`, `Magenta`
- `Teal`, `Gold`, `DarkRed`, `DarkGreen`, `DarkBlue`

### DiscordWebhookMessage

The message model for Discord webhooks.

```csharp
var message = new DiscordWebhookMessage
{
    Content = "Message content",
    Username = "Custom Username",
    AvatarUrl = "https://example.com/avatar.png",
    Tts = false,
    Embeds = [embed1, embed2]
};
```

## Examples

### Simple Message
```csharp
var message = new DiscordWebhookMessage
{
    Content = "Hello, Discord!"
};

var client = new DiscordWebhookClient();
await client.SendAsync(webhookUrl, message);
```

### Rich Embed with Custom Color
```csharp
var embed = new DiscordEmbedBuilder()
    .WithTitle("Alert")
    .WithDescription("This is an important notification")
    .WithColor("#FF0000") // Red color
    .Build();

var message = new DiscordWebhookMessage
{
    Embeds = [embed]
};
```

### Multiple Fields
```csharp
var embed = new DiscordEmbedBuilder()
    .WithTitle("System Status")
    .AddField("CPU Usage", "45%", true)
    .AddField("Memory Usage", "67%", true)
    .AddField("Disk Space", "23%", true)
    .AddField("Notes", "All systems operational", false)
    .Build();
```

## Error Handling

The library throws appropriate exceptions for common errors:

```csharp
try
{
    await client.SendAsync(webhookUrl, message);
}
catch (ArgumentException ex)
{
    // Invalid webhook URL or message
    Console.WriteLine($"Invalid input: {ex.Message}");
}
catch (HttpRequestException ex)
{
    // HTTP request failed
    Console.WriteLine($"Request failed: {ex.Message}");
}
```

## Requirements

- .NET 8.0 or later
- Internet connection for webhook requests

## License

This project is licensed under the MIT License.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
