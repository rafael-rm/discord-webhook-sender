using System.Net;
using System.Text;
using System.Text.Json;
using DiscordWebhookSender.Core;
using DiscordWebhookSender.Core.Models;
using Moq;
using Moq.Protected;

namespace DiscordWebhookSender.Tests;

public class DiscordWebhookClientIntegrationTests
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    private DiscordWebhookClient CreateClient(Mock<HttpMessageHandler> mockHandler)
    {
        // Reset the singleton instance before creating a new one
        DiscordWebhookClient.Reset();
        
        var httpClient = new HttpClient(mockHandler.Object);
        return DiscordWebhookClient.Get(httpClient);
    }

    [Fact]
    public async Task SendAsync_WithValidMessage_ShouldSendSuccessfully()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        var client = CreateClient(mockHandler);
        
        var webhookUrl = "https://discord.com/api/webhooks/123456789/abcdef";
        var message = new DiscordWebhookMessageBuilder()
            .WithContent("Test message")
            .WithUsername("TestBot")
            .Build();

        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

        // Act
        await client.SendAsync(webhookUrl, message);

        // Assert
        mockHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post &&
                req.RequestUri!.ToString() == webhookUrl &&
                req.Content!.Headers.ContentType!.MediaType == "application/json"),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task SendAsync_WhenDiscordReturnsError_ShouldThrowHttpRequestException()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        var client = CreateClient(mockHandler);
        
        var webhookUrl = "https://discord.com/api/webhooks/123456789/abcdef";
        var message = new DiscordWebhookMessageBuilder()
            .WithContent("Test message")
            .Build();

        var errorResponse = new { error = "Invalid webhook URL" };
        var errorContent = JsonSerializer.Serialize(errorResponse);

        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(errorContent, Encoding.UTF8, "application/json")
            });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(
            () => client.SendAsync(webhookUrl, message));

        // Verify that HttpRequestException is thrown (expected behavior)
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task SendAsync_WhenDiscordReturnsServerError_ShouldThrowHttpRequestException()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        var client = CreateClient(mockHandler);
        
        var webhookUrl = "https://discord.com/api/webhooks/123456789/abcdef";
        var message = new DiscordWebhookMessageBuilder()
            .WithContent("Test message")
            .Build();

        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("Internal Server Error", Encoding.UTF8, "text/plain")
            });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(
            () => client.SendAsync(webhookUrl, message));

        // Verify that HttpRequestException is thrown (expected behavior)
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task SendAsync_WhenHttpClientThrowsException_ShouldPropagateException()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        var client = CreateClient(mockHandler);
        
        var webhookUrl = "https://discord.com/api/webhooks/123456789/abcdef";
        var message = new DiscordWebhookMessageBuilder()
            .WithContent("Test message")
            .Build();

        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(
            () => client.SendAsync(webhookUrl, message));

        // Verify that the original exception is propagated
        Assert.Equal("Network error", exception.Message);
    }

    [Fact]
    public async Task SendAsync_WithCancellationToken_ShouldRespectCancellation()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        var client = CreateClient(mockHandler);
        
        var webhookUrl = "https://discord.com/api/webhooks/123456789/abcdef";
        var message = new DiscordWebhookMessageBuilder()
            .WithContent("Test message")
            .Build();

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

        // Act & Assert
        await Assert.ThrowsAnyAsync<TaskCanceledException>(
            () => client.SendAsync(webhookUrl, message, cancellationTokenSource.Token));
    }

    [Fact]
    public async Task SendAsync_WithJsonSerialization_ShouldUseCorrectFormat()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        var client = CreateClient(mockHandler);
        
        var webhookUrl = "https://discord.com/api/webhooks/123456789/abcdef";
        var message = new DiscordWebhookMessageBuilder()
            .WithContent("Test content")
            .WithUsername("TestBot")
            .WithTts(true)
            .WithAvatarUrl("https://example.com/avatar.png")
            .Build();

        string? capturedContent = null;

        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, token) =>
            {
                capturedContent = request.Content?.ReadAsStringAsync(token).Result;
            })
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

        // Act
        await client.SendAsync(webhookUrl, message);

        // Assert
        Assert.NotNull(capturedContent);
        var deserializedMessage = JsonSerializer.Deserialize<DiscordWebhookMessage>(capturedContent, _jsonOptions);
        Assert.Equal("Test content", deserializedMessage!.Content);
        Assert.Equal("TestBot", deserializedMessage.Username);
        Assert.True(deserializedMessage.Tts);
        Assert.Equal("https://example.com/avatar.png", deserializedMessage.AvatarUrl);
    }
} 