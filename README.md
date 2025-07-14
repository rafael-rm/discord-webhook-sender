# Discord Webhook Sender
[![NuGet Version](https://img.shields.io/nuget/v/DiscordWebhookSender.svg?style=flate&logo=nuget)](https://www.nuget.org/packages/DiscordWebhookSender)
[![NuGet Downloads](https://img.shields.io/nuget/dt/DiscordWebhookSender.svg?style=)](https://www.nuget.org/packages/DiscordWebhookSender)

A .NET library for sending Discord webhooks with rich embeds and message formatting. This library provides a simple and intuitive API for creating and sending Discord webhook messages with support for embeds, colors, fields, and more.

## Features

- **Simple Webhook Client**: Easy-to-use HTTP client for sending Discord webhooks
- **Rich Embed Builder**: Fluent API for creating beautiful Discord embeds
- **Message Builder**: Fluent API for creating Discord webhook messages with content, embeds, and customization
- **Color Support**: Built-in color constants and hex color support
- **Message Customization**: Support for username, avatar, content, and TTS
- **Field Management**: Add inline and block fields to embeds
- **Media Support**: Add images and thumbnails to embeds
- **Author & Footer**: Customize embed author and footer information
- **Timestamp Support**: Add timestamps to embeds
- **Singleton Webhook Client**: Thread-safe singleton pattern to ensure only one HTTP client instance
- **Discord API Validation**: Automatic validation of all Discord API limits and constraints
- **Comprehensive Error Handling**: Detailed error messages for validation failures

## Installation

```bash
dotnet add package DiscordWebhookSender
```

## Quick Start

### Simple Text Message
```csharp
using DiscordWebhookSender;

var client = DiscordWebhookClient.Get();
await client.SendAsync("YOUR_DISCORD_WEBHOOK_URL", "Hello, Discord!");
```

### Simple Embed
```csharp
using DiscordWebhookSender;
using DiscordWebhookSender.Models;

var embed = new DiscordEmbedBuilder()
    .WithTitle("🚀 Deploy Completed!")
    .WithDescription("The new application version has been published.")
    .WithColor(DiscordEmbedColor.Green)
    .Build();

var client = DiscordWebhookClient.Get();
await client.SendAsync("YOUR_DISCORD_WEBHOOK_URL", embed);
```

### Advanced Message with Customization
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

// Create the webhook message using the builder
var message = new DiscordWebhookMessageBuilder()
    .WithContent("✅ Automatic update completed")
    .WithUsername("CI/CD Bot")
    .WithAvatarUrl("https://example.com/avatar.png")
    .AddEmbed(embed)
    .Build();

// Send the webhook
var client = DiscordWebhookClient.Get();
await client.SendAsync("YOUR_DISCORD_WEBHOOK_URL", message);
```

## API Reference

### DiscordWebhookClient

The main client for sending Discord webhooks. Implements the Singleton pattern to ensure only one instance exists.

#### Usage
```csharp
var client = DiscordWebhookClient.Get();
```

#### Methods

```csharp
// Send a complete DiscordWebhookMessage
await client.SendAsync(webhookUrl, message, cancellationToken);

// Send a simple text message
await client.SendAsync(webhookUrl, content, cancellationToken);

// Send a single embed
await client.SendAsync(webhookUrl, embed, cancellationToken);
```

### DiscordEmbedBuilder

Fluent API for building Discord embeds with automatic validation of Discord API limits.

#### Basic Methods
- `WithTitle(string title)` - Set the embed title (max 256 characters)
- `WithDescription(string description)` - Set the embed description (max 4096 characters)
- `WithUrl(string url)` - Set the embed URL

#### Timestamp Methods
- `WithTimestamp(DateTimeOffset timestamp)` - Set the embed timestamp using DateTimeOffset
- `WithTimestamp(DateTime dateTime)` - Set the embed timestamp using DateTime (converted to DateTimeOffset)

#### Color Methods
- `WithColor(int color)` - Set color using integer value
- `WithColor(DiscordEmbedColor color)` - Set color using predefined enum
- `WithColor(string hexColor)` - Set color using hex string (e.g., "#FF0000")

#### Media Methods
- `WithImage(string url)` - Add an image to the embed
- `WithThumbnail(string url)` - Add a thumbnail to the embed

#### Author & Footer
- `WithAuthor(string name, string? url, string? iconUrl)` - Set embed author (name max 256 characters)
- `WithFooter(string text, string? iconUrl)` - Set embed footer (text max 2048 characters)

#### Fields
- `AddField(string name, string value, bool inline)` - Add a field to the embed (name max 256, value max 1024, max 25 fields)

### DiscordEmbedColor Enum

Predefined colors for easy use:
- `Default`, `White`, `Red`, `Green`, `Blue`
- `Yellow`, `Orange`, `Purple`, `Cyan`, `Magenta`
- `Teal`, `Gold`, `DarkRed`, `DarkGreen`, `DarkBlue`
- `Navy`, `Olive`, `Pink`, `DarkPink`

### DiscordWebhookMessageBuilder

Fluent API for building Discord webhook messages with automatic validation of Discord API limits.

#### Basic Methods
- `WithContent(string content)` - Set the message content (max 2000 characters)
- `WithUsername(string username)` - Set the custom username (max 80 characters)
- `WithTts(bool tts)` - Enable or disable text-to-speech
- `WithAvatarUrl(string avatarUrl)` - Set the custom avatar URL

#### Embed Methods
- `AddEmbed(DiscordEmbed embed)` - Add a single embed to the message
- `AddEmbed(IEnumerable<DiscordEmbed> embeds)` - Add multiple embeds from a collection
- `AddEmbed(params DiscordEmbed[] embeds)` - Add multiple embeds using params
- `ClearEmbeds()` - Remove all embeds from the message

#### Usage Example
```csharp
var message = new DiscordWebhookMessageBuilder()
    .WithContent("Hello, Discord!")
    .WithUsername("My Bot")
    .WithAvatarUrl("https://example.com/avatar.png")
    .WithTts(false)
    .AddEmbed(embed1)
    .AddEmbed(embed2, embed3)
    .Build();
```

### DiscordWebhookMessage

The message model for Discord webhooks with automatic validation.

```csharp
var message = new DiscordWebhookMessage
{
    Content = "Message content", // max 2000 characters
    Username = "Custom Username", // max 80 characters
    AvatarUrl = "https://example.com/avatar.png",
    Tts = false,
    Embeds = [embed1, embed2] // max 10 embeds
};
```

## Validation and Error Handling

The library automatically validates all Discord API limits before sending messages. If any validation fails, a `DiscordValidationException` is thrown with detailed error information.

### Discord API Limits

- **Message Content**: Maximum 2000 characters
- **Username**: Maximum 80 characters
- **Embed Title**: Maximum 256 characters
- **Embed Description**: Maximum 4096 characters
- **Field Name**: Maximum 256 characters
- **Field Value**: Maximum 1024 characters
- **Footer Text**: Maximum 2048 characters
- **Author Name**: Maximum 256 characters
- **Fields per Embed**: Maximum 25 fields
- **Embeds per Message**: Maximum 10 embeds
- **Total Embed Content**: Maximum 6000 characters (sum of all text content)

### Error Handling Example

```csharp
try
{
    var embed = new DiscordEmbedBuilder()
        .WithTitle("This title is way too long and will exceed the 256 character limit that Discord imposes on embed titles, causing a validation exception to be thrown when the Build() method is called or when the message is sent, which means that any attempt to send this embed with such a title will fail immediately due to the hard limit enforced by the Discord API. This serves as an example of what *not* to do when setting the title of a Discord embed. Always make sure to validate title length before assigning it.")
        .Build();
}
catch (DiscordValidationException ex)
{
    Console.WriteLine($"Validation failed: {ex.Message}");
    // Output: Validation failed: Embed title exceeds the maximum length of 256 characters. Current length: 502 characters.
}
```

## Examples

### Simple Text Message
```csharp
var client = DiscordWebhookClient.Get();
await client.SendAsync(webhookUrl, "Hello, Discord!");
```

### Simple Embed
```csharp
var embed = new DiscordEmbedBuilder()
    .WithTitle("Alert")
    .WithDescription("This is an important notification")
    .WithColor("#FF0000") // Red color
    .Build();

var client = DiscordWebhookClient.Get();
await client.SendAsync(webhookUrl, embed);
```

### Complete Message with Multiple Embeds
```csharp
var embed1 = new DiscordEmbedBuilder()
    .WithTitle("System Status")
    .WithColor(DiscordEmbedColor.Green)
    .Build();

var embed2 = new DiscordEmbedBuilder()
    .WithTitle("Performance Metrics")
    .AddField("CPU Usage", "45%", true)
    .AddField("Memory Usage", "67%", true)
    .WithColor(DiscordEmbedColor.Blue)
    .Build();

var message = new DiscordWebhookMessageBuilder()
    .WithContent("📊 System Report")
    .WithUsername("System Monitor")
    .AddEmbed(embed1, embed2)
    .Build();

var client = DiscordWebhookClient.Get();
await client.SendAsync(webhookUrl, message);
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

### Timestamp Examples
```csharp
// Using DateTimeOffset (recommended for UTC timestamps)
var embed1 = new DiscordEmbedBuilder()
    .WithTitle("Event Log")
    .WithTimestamp(DateTimeOffset.UtcNow)
    .Build();

// Using DateTime (automatically converted to DateTimeOffset)
var embed2 = new DiscordEmbedBuilder()
    .WithTitle("Local Event")
    .WithTimestamp(DateTime.Now)
    .Build();
```

## Requirements

- .NET 8.0 or .NET 9.0
- Internet connection for webhook requests

## License

This project is licensed under the MIT License.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
