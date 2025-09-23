using System.Text.Json;
using DiscordWebhookSender.Core.DTO;

namespace DiscordWebhookSender.Tests;

public class DiscordRateLimitResponseTests
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    [Fact]
    public void DiscordRateLimitResponse_ShouldDeserializeCorrectly()
    {
        // Arrange
        var json = """
        {
            "message": "You are being rate limited.",
            "retry_after": 1.5,
            "global": true
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<DiscordRateLimitResponse>(json, _jsonOptions);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("You are being rate limited.", response.Message);
        Assert.Equal(1.5, response.RetryAfterSeconds);
        Assert.True(response.IsGlobal);
    }

    [Fact]
    public void DiscordRateLimitResponse_ShouldDeserializeWithZeroRetryAfter()
    {
        // Arrange
        var json = """
        {
            "message": "Rate limit exceeded",
            "retry_after": 0,
            "global": false
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<DiscordRateLimitResponse>(json, _jsonOptions);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("Rate limit exceeded", response.Message);
        Assert.Equal(0, response.RetryAfterSeconds);
        Assert.False(response.IsGlobal);
    }

    [Fact]
    public void DiscordRateLimitResponse_ShouldDeserializeWithMissingFields()
    {
        // Arrange
        var json = "{}";

        // Act
        var response = JsonSerializer.Deserialize<DiscordRateLimitResponse>(json, _jsonOptions);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(string.Empty, response.Message);
        Assert.Equal(0, response.RetryAfterSeconds);
        Assert.False(response.IsGlobal);
    }

    [Fact]
    public void DiscordRateLimitResponse_ShouldSerializeCorrectly()
    {
        // Arrange
        var response = new DiscordRateLimitResponse
        {
            Message = "Rate limit exceeded",
            RetryAfterSeconds = 2.5,
            IsGlobal = true
        };

        // Act
        var json = JsonSerializer.Serialize(response, _jsonOptions);

        // Assert
        Assert.Contains("\"message\":\"Rate limit exceeded\"", json);
        Assert.Contains("\"retry_after\":2.5", json);
        Assert.Contains("\"global\":true", json);
    }

    [Fact]
    public void DiscordRateLimitResponse_ShouldHandleNegativeRetryAfter()
    {
        // Arrange
        var json = """
        {
            "message": "Invalid retry time",
            "retry_after": -1.0,
            "global": false
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<DiscordRateLimitResponse>(json, _jsonOptions);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("Invalid retry time", response.Message);
        Assert.Equal(-1.0, response.RetryAfterSeconds);
        Assert.False(response.IsGlobal);
    }

    [Fact]
    public void DiscordRateLimitResponse_ShouldHandleLargeRetryAfter()
    {
        // Arrange
        var json = """
        {
            "message": "Global rate limit",
            "retry_after": 3600.0,
            "global": true
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<DiscordRateLimitResponse>(json, _jsonOptions);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("Global rate limit", response.Message);
        Assert.Equal(3600.0, response.RetryAfterSeconds);
        Assert.True(response.IsGlobal);
    }
}
