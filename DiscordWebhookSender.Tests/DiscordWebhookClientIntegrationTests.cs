using System.Net;
using System.Text;
using System.Text.Json;
using DiscordWebhookSender.Core;
using DiscordWebhookSender.Core.DTO;
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
        var httpClient = new HttpClient(mockHandler.Object);
        return DiscordWebhookClient.Create(httpClient: httpClient);
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

    [Fact]
    public async Task SendAsync_WithRateLimitEnabled_ShouldRetryAfterDelay()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        var client = CreateClient(mockHandler);
        
        var webhookUrl = "https://discord.com/api/webhooks/123456789/abcdef";
        var message = new DiscordWebhookMessageBuilder()
            .WithContent("Test message")
            .Build();

        var rateLimitResponse = new DiscordRateLimitResponse
        {
            Message = "You are being rate limited.",
            RetryAfterSeconds = 0.1, // 100ms for quick test
            IsGlobal = false
        };

        var rateLimitJson = JsonSerializer.Serialize(rateLimitResponse, _jsonOptions);

        var callCount = 0;
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                callCount++;
                if (callCount == 1)
                {
                    return new HttpResponseMessage(HttpStatusCode.TooManyRequests)
                    {
                        Content = new StringContent(rateLimitJson, Encoding.UTF8, "application/json")
                    };
                }
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            });

        // Act
        var startTime = DateTime.UtcNow;
        await client.SendAsync(webhookUrl, message);
        var endTime = DateTime.UtcNow;

        // Assert
        Assert.Equal(2, callCount); // First call returns 429, second returns success
        Assert.True(endTime - startTime >= TimeSpan.FromMilliseconds(100)); // Should have waited at least 100ms
    }

    [Fact]
    public async Task SendAsync_WithRateLimitDisabled_ShouldNotRetryOn429()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        
        var httpClient = new HttpClient(mockHandler.Object);
        var client = DiscordWebhookClient.Create(enableRateLimitHandling: false, httpClient: httpClient);
        
        var webhookUrl = "https://discord.com/api/webhooks/123456789/abcdef";
        var message = new DiscordWebhookMessageBuilder()
            .WithContent("Test message")
            .Build();

        var rateLimitResponse = new DiscordRateLimitResponse
        {
            Message = "You are being rate limited.",
            RetryAfterSeconds = 1.0,
            IsGlobal = false
        };

        var rateLimitJson = JsonSerializer.Serialize(rateLimitResponse, _jsonOptions);

        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.TooManyRequests)
            {
                Content = new StringContent(rateLimitJson, Encoding.UTF8, "application/json")
            });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(
            () => client.SendAsync(webhookUrl, message));

        Assert.Contains("429", exception.Message);
    }

    [Fact]
    public async Task SendAsync_WithGlobalRateLimit_ShouldHandleCorrectly()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        var client = CreateClient(mockHandler);
        
        var webhookUrl = "https://discord.com/api/webhooks/123456789/abcdef";
        var message = new DiscordWebhookMessageBuilder()
            .WithContent("Test message")
            .Build();

        var rateLimitResponse = new DiscordRateLimitResponse
        {
            Message = "You are being globally rate limited.",
            RetryAfterSeconds = 0.1,
            IsGlobal = true
        };

        var rateLimitJson = JsonSerializer.Serialize(rateLimitResponse, _jsonOptions);

        var callCount = 0;
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                callCount++;
                if (callCount == 1)
                {
                    return new HttpResponseMessage(HttpStatusCode.TooManyRequests)
                    {
                        Content = new StringContent(rateLimitJson, Encoding.UTF8, "application/json")
                    };
                }
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            });

        // Act
        await client.SendAsync(webhookUrl, message);

        // Assert
        Assert.Equal(2, callCount); // Should have returned after delay
    }

    [Fact]
    public async Task SendAsync_WithRateLimitAndMaxRetries_ShouldEventuallyFail()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        
        var httpClient = new HttpClient(mockHandler.Object);
        var client = DiscordWebhookClient.Create(enableRateLimitHandling: true, maxRetries: 2, httpClient: httpClient);
        
        var webhookUrl = "https://discord.com/api/webhooks/123456789/abcdef";
        var message = new DiscordWebhookMessageBuilder()
            .WithContent("Test message")
            .Build();

        var rateLimitResponse = new DiscordRateLimitResponse
        {
            Message = "You are being rate limited.",
            RetryAfterSeconds = 0.1,
            IsGlobal = false
        };

        var rateLimitJson = JsonSerializer.Serialize(rateLimitResponse, _jsonOptions);

        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => new HttpResponseMessage(HttpStatusCode.TooManyRequests)
            {
                Content = new StringContent(rateLimitJson, Encoding.UTF8, "application/json")
            });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(
            () => client.SendAsync(webhookUrl, message));

        Assert.Contains("Failed to send Discord webhook after 2 attempts", exception.Message);
    }

    [Fact]
    public async Task SendAsync_WithRateLimitAndCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        var client = CreateClient(mockHandler);
        
        var webhookUrl = "https://discord.com/api/webhooks/123456789/abcdef";
        var message = new DiscordWebhookMessageBuilder()
            .WithContent("Test message")
            .Build();

        var rateLimitResponse = new DiscordRateLimitResponse
        {
            Message = "You are being rate limited.",
            RetryAfterSeconds = 5.0, // 5 seconds to ensure cancellation
            IsGlobal = false
        };

        var rateLimitJson = JsonSerializer.Serialize(rateLimitResponse, _jsonOptions);

        var callCount = 0;
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                callCount++;
                if (callCount == 1)
                {
                    return new HttpResponseMessage(HttpStatusCode.TooManyRequests)
                    {
                        Content = new StringContent(rateLimitJson, Encoding.UTF8, "application/json")
                    };
                }
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            });

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(50)); // Cancel after 50ms

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => client.SendAsync(webhookUrl, message, cancellationTokenSource.Token));
    }

    [Fact]
    public async Task SendAsync_WithInvalidRateLimitResponse_ShouldNotRetry()
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
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.TooManyRequests)
            {
                Content = new StringContent("Invalid JSON", Encoding.UTF8, "application/json")
            });

        // Act & Assert
        var exception = await Assert.ThrowsAnyAsync<Exception>(
            () => client.SendAsync(webhookUrl, message));

        // Deve falhar com JsonException ao tentar deserializar o JSON inválido
        Assert.True(exception is JsonException || exception is HttpRequestException);
    }

    [Fact]
    public async Task SendAsync_WithZeroRetryAfter_ShouldNotDelay()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        var client = CreateClient(mockHandler);
        
        var webhookUrl = "https://discord.com/api/webhooks/123456789/abcdef";
        var message = new DiscordWebhookMessageBuilder()
            .WithContent("Test message")
            .Build();

        var rateLimitResponse = new DiscordRateLimitResponse
        {
            Message = "Rate limit exceeded",
            RetryAfterSeconds = 0, // Zero delay
            IsGlobal = false
        };

        var rateLimitJson = JsonSerializer.Serialize(rateLimitResponse, _jsonOptions);

        var callCount = 0;
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                callCount++;
                if (callCount == 1)
                {
                    return new HttpResponseMessage(HttpStatusCode.TooManyRequests)
                    {
                        Content = new StringContent(rateLimitJson, Encoding.UTF8, "application/json")
                    };
                }
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            });

        // Act
        var startTime = DateTime.UtcNow;
        await client.SendAsync(webhookUrl, message);
        var endTime = DateTime.UtcNow;

        // Assert
        Assert.Equal(2, callCount);
        Assert.True(endTime - startTime < TimeSpan.FromMilliseconds(100)); // Should be very fast
    }
}