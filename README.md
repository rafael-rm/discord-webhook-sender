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
- **Shared HttpClient**: Uses a shared HttpClient instance to prevent socket exhaustion
- **Instance-Based Architecture**: Create multiple client instances as needed without singleton constraints
- **Discord API Validation**: Automatic validation of all Discord API limits and constraints
- **Comprehensive Error Handling**: Detailed error messages for validation failures
- **Automatic Rate Limit Handling**: Built-in support for Discord rate limiting with automatic retry and delay
- **Configurable Rate Limit Behavior**: Option to enable or disable automatic rate limit handling
- **Configurable Retry Logic**: Customizable maximum number of retries for transient failures

## Installation

```bash
dotnet add package DiscordWebhookSender
```

## Architecture Changes

This library has been updated to use an instance-based architecture instead of the singleton pattern. This change provides several benefits:

- **Multiple Instances**: Create multiple client instances with different configurations
- **Better Testability**: Easier to test with different configurations and mock objects
- **Shared HttpClient**: Prevents socket exhaustion by using a shared HttpClient instance
- **Flexibility**: Each instance can have its own HttpClient or use the shared one
- **No Global State**: Eliminates potential issues with global state management

### Migration from Singleton Pattern

If you were using the previous singleton pattern, update your code as follows:

```csharp
// Old way (singleton)
var client = DiscordWebhookClient.Get();

// New way (instance-based)
var client = DiscordWebhookClient.Create();
```

The API remains the same, only the creation method has changed.

## Quick Start

### Simple Text Message
```csharp
using DiscordWebhookSender;

var client = DiscordWebhookClient.Create();
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

var client = DiscordWebhookClient.Create();
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
var client = DiscordWebhookClient.Create();
await client.SendAsync("YOUR_DISCORD_WEBHOOK_URL", message);
```

## API Reference

### DiscordWebhookClient

The main client for sending Discord webhooks. Uses a shared HttpClient to prevent socket exhaustion.

#### Usage
```csharp
// Basic usage with default settings (rate limit handling enabled, 3 retries)
var client = DiscordWebhookClient.Create();

// With custom HttpClient
var httpClient = new HttpClient();
var client = DiscordWebhookClient.Create(httpClient);

// With rate limit handling disabled
var client = DiscordWebhookClient.Create(enableRateLimitHandling: false);

// With custom retry count
var client = DiscordWebhookClient.Create(maxRetries: 5);

// With custom HttpClient, rate limit handling disabled, and custom retry count
var httpClient = new HttpClient();
var client = DiscordWebhookClient.Create(httpClient, enableRateLimitHandling: false, maxRetries: 2);

// All parameters customized
var httpClient = new HttpClient();
var client = DiscordWebhookClient.Create(httpClient, enableRateLimitHandling: true, maxRetries: 10);
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

#### Methods
- `WithTitle(string title)` - Set the embed title (max 256 characters)
- `WithDescription(string description)` - Set the embed description (max 4096 characters)
- `WithUrl(string url)` - Set the embed URL
- `WithTimestamp(DateTimeOffset timestamp)` - Set the embed timestamp using DateTimeOffset
- `WithTimestamp(DateTime dateTime)` - Set the embed timestamp using DateTime (converted to DateTimeOffset)
- `WithColor(int color)` - Set color using integer value
- `WithColor(DiscordEmbedColor color)` - Set color using predefined enum
- `WithColor(string hexColor)` - Set color using hex string (e.g., "#FF0000")
- `WithImage(string url)` - Add an image to the embed
- `WithThumbnail(string url)` - Add a thumbnail to the embed
- `WithAuthor(string name, string? url, string? iconUrl)` - Set embed author (name max 256 characters)
- `WithFooter(string text, string? iconUrl)` - Set embed footer (text max 2048 characters)
- `AddField(string name, string value, bool inline)` - Add a field to the embed (name max 256, value max 1024, max 25 fields)

### DiscordEmbedColor Enum

Predefined colors for easy use:
- `Default`, `White`, `Red`, `Green`, `Blue`
- `Yellow`, `Orange`, `Purple`, `Cyan`, `Magenta`
- `Teal`, `Gold`, `DarkRed`, `DarkGreen`, `DarkBlue`
- `Navy`, `Olive`, `Pink`, `DarkPink`

### DiscordWebhookMessageBuilder

Fluent API for building Discord webhook messages with automatic validation of Discord API limits.

#### Methods
- `WithContent(string content)` - Set the message content (max 2000 characters)
- `WithUsername(string username)` - Set the custom username (max 80 characters)
- `WithTts(bool tts)` - Enable or disable text-to-speech
- `WithAvatarUrl(string avatarUrl)` - Set the custom avatar URL
- `AddEmbed(DiscordEmbed embed)` - Add a single embed to the message
- `AddEmbed(IEnumerable<DiscordEmbed> embeds)` - Add multiple embeds from a collection
- `AddEmbed(params DiscordEmbed[] embeds)` - Add multiple embeds using params
- `ClearEmbeds()` - Remove all embeds from the message

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

## Rate Limit Handling

The library includes built-in support for Discord's rate limiting system. When enabled, the client automatically handles HTTP 429 (Too Many Requests) responses by:

- **Automatic Detection**: Detects rate limit responses from Discord API
- **Intelligent Delays**: Waits for the exact time specified in Discord's `retry_after` header
- **Buffer Time**: Adds a small buffer (100ms) to ensure compliance with Discord's limits
- **Automatic Retry**: Retries the request after the delay period
- **Configurable**: Can be enabled or disabled based on your needs
- **Customizable Retry Count**: Configure the maximum number of retry attempts for transient failures

### Enabling/Disabling Rate Limit Handling

By default, rate limit handling is **enabled**. You can control this behavior when creating the client:

```csharp
// Rate limit handling enabled (default) with 3 retries (default)
var client = DiscordWebhookClient.Create();

// Rate limit handling disabled
var client = DiscordWebhookClient.Create(enableRateLimitHandling: false);

// Custom retry count with rate limit handling enabled
var client = DiscordWebhookClient.Create(maxRetries: 5);

// Rate limit handling disabled with custom retry count
var client = DiscordWebhookClient.Create(enableRateLimitHandling: false, maxRetries: 1);

// With custom HttpClient, rate limit handling disabled, and custom retry count
var httpClient = new HttpClient();
var client = DiscordWebhookClient.Create(httpClient, enableRateLimitHandling: false, maxRetries: 2);
```

### Retry Configuration

The library supports configurable retry logic for handling transient failures:

- **Default Retries**: 3 attempts by default
- **Custom Retry Count**: Set any number of retries (minimum 1)
- **Rate Limit Retries**: Rate limit handling works independently of the retry count
- **Failure Handling**: After exhausting all retries, throws `HttpRequestException`

```csharp
// Conservative approach - only 1 retry
var client = DiscordWebhookClient.Create(maxRetries: 1);

// Aggressive approach - 10 retries for high-reliability scenarios
var client = DiscordWebhookClient.Create(maxRetries: 10);

// No retries - fail fast on any error
var client = DiscordWebhookClient.Create(maxRetries: 0);
```

### How It Works

When a rate limit is encountered:

1. The client receives an HTTP 429 response from Discord
2. It parses the `retry_after` value from the response
3. Waits for the specified duration plus a 100ms buffer
4. Automatically retries the request
5. If rate limit handling is disabled, the request fails immediately with an `HttpRequestException`

For other transient failures (network issues, server errors):

1. The client attempts the request
2. If it fails with a transient error, it retries up to `maxRetries` times
3. If all retries are exhausted, throws `HttpRequestException`

### Rate Limit Response Structure

The library automatically parses Discord's rate limit response:

```json
{
  "message": "You are being rate limited.",
  "retry_after": 1.5,
  "global": false
}
```

- `message`: Description of the rate limit
- `retry_after`: Number of seconds to wait before retrying
- `global`: Whether this is a global rate limit affecting all Discord API calls

## Additional Examples

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

var client = DiscordWebhookClient.Create();
await client.SendAsync(webhookUrl, message);
```

### Multiple Fields and Timestamps
```csharp
var embed = new DiscordEmbedBuilder()
    .WithTitle("System Status")
    .AddField("CPU Usage", "45%", true)
    .AddField("Memory Usage", "67%", true)
    .AddField("Disk Space", "23%", true)
    .AddField("Notes", "All systems operational", false)
    .WithTimestamp(DateTimeOffset.UtcNow)
    .Build();
```

### Rate Limit Handling Examples

#### High-Volume Message Sending with Rate Limit Handling
```csharp
// Client with rate limit handling enabled (default)
var client = DiscordWebhookClient.Create();

// Send multiple messages - rate limits are handled automatically
for (int i = 0; i < 100; i++)
{
    var message = new DiscordWebhookMessageBuilder()
        .WithContent($"Message {i + 1}")
        .Build();
    
    // If rate limited, the client will automatically wait and retry
    await client.SendAsync(webhookUrl, message);
}
```

#### Disabling Rate Limit Handling for Custom Logic
```csharp
// Client with rate limit handling disabled
var client = DiscordWebhookClient.Create(enableRateLimitHandling: false);

try
{
    await client.SendAsync(webhookUrl, message);
}
catch (HttpRequestException ex) when (ex.Message.Contains("429"))
{
    // Handle rate limiting manually
    Console.WriteLine("Rate limited! Implementing custom retry logic...");
    await Task.Delay(TimeSpan.FromSeconds(5));
    // Retry or implement custom logic
}
```

#### Custom Retry Configuration Examples

#### High-Reliability Setup with Many Retries
```csharp
// Client configured for high-reliability scenarios
var client = DiscordWebhookClient.Create(maxRetries: 10);

try
{
    await client.SendAsync(webhookUrl, message);
    Console.WriteLine("Message sent successfully!");
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Failed after 10 retries: {ex.Message}");
}
```

#### Fast-Fail Setup for Real-Time Applications
```csharp
// Client configured to fail fast (no retries)
var client = DiscordWebhookClient.Create(maxRetries: 0);

try
{
    await client.SendAsync(webhookUrl, message);
}
catch (HttpRequestException ex)
{
    // Handle failure immediately - no retries attempted
    Console.WriteLine("Message failed immediately: " + ex.Message);
    // Implement fallback logic or user notification
}
```

#### Conservative Retry Strategy
```csharp
// Conservative approach - only 1 retry attempt
var client = DiscordWebhookClient.Create(maxRetries: 1);

// This will attempt the request once, and if it fails, retry once more
await client.SendAsync(webhookUrl, message);
```

#### Production Environment Configuration
```csharp
// Production setup with custom HttpClient and balanced retry strategy
var httpClient = new HttpClient();
httpClient.Timeout = TimeSpan.FromSeconds(30);

var client = DiscordWebhookClient.Create(
    httpClient: httpClient,
    enableRateLimitHandling: true,
    maxRetries: 5
);

// This configuration provides:
// - Custom timeout for HTTP requests
// - Rate limit handling enabled
// - 5 retry attempts for transient failures
// - Automatic rate limit compliance
```

## Requirements

- .NET **10.0, 9.0, or 8.0** (any of these versions)
- Internet connection for webhook requests

## License

This project is licensed under the MIT License.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
