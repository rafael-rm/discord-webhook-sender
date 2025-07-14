using DiscordWebhookSender.Core;
using DiscordWebhookSender.Core.Exceptions;
using DiscordWebhookSender.Core.Models;
using DiscordWebhookSender.Core.Validation;

namespace DiscordWebhookSender.Tests;

public class DiscordWebhookMessageBuilderTests
{
    [Fact]
    public void Build_WithContent_ShouldCreateMessageWithContent()
    {
        // Arrange
        var content = "Test message content";
        
        // Act
        var message = new DiscordWebhookMessageBuilder()
            .WithContent(content)
            .Build();
        
        // Assert
        Assert.Equal(content, message.Content);
    }

    [Fact]
    public void Build_WithUsername_ShouldCreateMessageWithUsername()
    {
        // Arrange
        var username = "TestBot";
        
        // Act
        var message = new DiscordWebhookMessageBuilder()
            .WithUsername(username)
            .Build();
        
        // Assert
        Assert.Equal(username, message.Username);
    }

    [Fact]
    public void Build_WithTts_ShouldCreateMessageWithTts()
    {
        // Arrange
        var tts = true;
        
        // Act
        var message = new DiscordWebhookMessageBuilder()
            .WithTts(tts)
            .Build();
        
        // Assert
        Assert.Equal(tts, message.Tts);
    }

    [Fact]
    public void Build_WithAvatarUrl_ShouldCreateMessageWithAvatarUrl()
    {
        // Arrange
        var avatarUrl = "https://example.com/avatar.png";
        
        // Act
        var message = new DiscordWebhookMessageBuilder()
            .WithAvatarUrl(avatarUrl)
            .Build();
        
        // Assert
        Assert.Equal(avatarUrl, message.AvatarUrl);
    }

    [Fact]
    public void Build_WithSingleEmbed_ShouldCreateMessageWithEmbed()
    {
        // Arrange
        var embed = new DiscordEmbedBuilder()
            .WithTitle("Test Title")
            .WithDescription("Test Description")
            .Build();
        
        // Act
        var message = new DiscordWebhookMessageBuilder()
            .AddEmbed(embed)
            .Build();
        
        // Assert
        Assert.NotNull(message.Embeds);
        Assert.Single(message.Embeds);
        Assert.Equal("Test Title", message.Embeds[0].Title);
        Assert.Equal("Test Description", message.Embeds[0].Description);
    }

    [Fact]
    public void Build_WithMultipleEmbeds_ShouldCreateMessageWithAllEmbeds()
    {
        // Arrange
        var embed1 = new DiscordEmbedBuilder()
            .WithTitle("Embed 1")
            .Build();
        
        var embed2 = new DiscordEmbedBuilder()
            .WithTitle("Embed 2")
            .Build();
        
        // Act
        var message = new DiscordWebhookMessageBuilder()
            .AddEmbed(embed1, embed2)
            .Build();
        
        // Assert
        Assert.NotNull(message.Embeds);
        Assert.Equal(2, message.Embeds.Count);
        Assert.Equal("Embed 1", message.Embeds[0].Title);
        Assert.Equal("Embed 2", message.Embeds[1].Title);
    }

    [Fact]
    public void Build_WithEmbedsCollection_ShouldCreateMessageWithAllEmbeds()
    {
        // Arrange
        var embeds = new List<DiscordEmbed>
        {
            new DiscordEmbedBuilder().WithTitle("Embed 1").Build(),
            new DiscordEmbedBuilder().WithTitle("Embed 2").Build(),
            new DiscordEmbedBuilder().WithTitle("Embed 3").Build()
        };
        
        // Act
        var message = new DiscordWebhookMessageBuilder()
            .AddEmbed(embeds)
            .Build();
        
        // Assert
        Assert.NotNull(message.Embeds);
        Assert.Equal(3, message.Embeds.Count);
        Assert.Equal("Embed 1", message.Embeds[0].Title);
        Assert.Equal("Embed 2", message.Embeds[1].Title);
        Assert.Equal("Embed 3", message.Embeds[2].Title);
    }

    [Fact]
    public void Build_WithCompleteMessage_ShouldCreateCompleteMessage()
    {
        // Arrange
        var embed = new DiscordEmbedBuilder()
            .WithTitle("Test Embed")
            .WithDescription("Test Description")
            .Build();
        
        // Act
        var message = new DiscordWebhookMessageBuilder()
            .WithContent("Test content")
            .WithUsername("TestBot")
            .WithTts(true)
            .WithAvatarUrl("https://example.com/avatar.png")
            .AddEmbed(embed)
            .Build();
        
        // Assert
        Assert.Equal("Test content", message.Content);
        Assert.Equal("TestBot", message.Username);
        Assert.True(message.Tts);
        Assert.Equal("https://example.com/avatar.png", message.AvatarUrl);
        Assert.NotNull(message.Embeds);
        Assert.Single(message.Embeds);
        Assert.Equal("Test Embed", message.Embeds[0].Title);
    }

    [Fact]
    public void Build_WithContentTooLong_ShouldThrowValidationException()
    {
        // Arrange
        var longContent = new string('A', DiscordLimits.MaxContentLength + 1);
        
        // Act & Assert
        var exception = Assert.Throws<DiscordValidationException>(() =>
            new DiscordWebhookMessageBuilder()
                .WithContent(longContent)
                .Build());
        
        Assert.Contains("exceeds the maximum length", exception.Message);
    }

    [Fact]
    public void Build_WithUsernameTooLong_ShouldThrowValidationException()
    {
        // Arrange
        var longUsername = new string('A', DiscordLimits.MaxUsernameLength + 1);
        
        // Act & Assert
        var exception = Assert.Throws<DiscordValidationException>(() =>
            new DiscordWebhookMessageBuilder()
                .WithUsername(longUsername)
                .Build());
        
        Assert.Contains("exceeds the maximum length", exception.Message);
    }

    [Fact]
    public void Build_WithTooManyEmbeds_ShouldThrowValidationException()
    {
        // Arrange
        var embeds = new List<DiscordEmbed>();
        for (int i = 0; i < DiscordLimits.MaxEmbedsPerMessage + 1; i++)
        {
            embeds.Add(new DiscordEmbedBuilder().WithTitle($"Embed {i}").Build());
        }
        
        // Act & Assert
        var exception = Assert.Throws<DiscordValidationException>(() =>
            new DiscordWebhookMessageBuilder()
                .AddEmbed(embeds)
                .Build());
        
        Assert.Contains("too many embeds", exception.Message);
    }

    [Fact]
    public void ClearEmbeds_ShouldRemoveAllEmbeds()
    {
        // Arrange
        var embed = new DiscordEmbedBuilder()
            .WithTitle("Test Embed")
            .Build();
        
        // Act
        var message = new DiscordWebhookMessageBuilder()
            .AddEmbed(embed)
            .ClearEmbeds()
            .Build();
        
        // Assert
        Assert.Null(message.Embeds);
    }

    [Fact]
    public void Build_WithEmptyMessage_ShouldCreateValidMessage()
    {
        // Act
        var message = new DiscordWebhookMessageBuilder().Build();
        
        // Assert
        Assert.NotNull(message);
        Assert.Null(message.Content);
        Assert.Null(message.Username);
        Assert.Null(message.Tts);
        Assert.Null(message.AvatarUrl);
        Assert.Null(message.Embeds);
    }

    [Fact]
    public void MethodChaining_ShouldWorkCorrectly()
    {
        // Act
        var message = new DiscordWebhookMessageBuilder()
            .WithContent("Content")
            .WithUsername("Username")
            .WithTts(false)
            .WithAvatarUrl("https://example.com/avatar.png")
            .Build();
        
        // Assert
        Assert.Equal("Content", message.Content);
        Assert.Equal("Username", message.Username);
        Assert.False(message.Tts);
        Assert.Equal("https://example.com/avatar.png", message.AvatarUrl);
    }
} 