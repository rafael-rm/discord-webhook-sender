using DiscordWebhookSender.Core;
using DiscordWebhookSender.Core.Models;
using DiscordWebhookSender.Core.Exceptions;
using DiscordWebhookSender.Core.Validation;

namespace DiscordWebhookSender.Tests;

public class DiscordEmbedBuilderTests
{
    [Fact]
    public void Build_WithAllFieldsFilled_ShouldCreateCompleteEmbed()
    {
        // Arrange
        var timestamp = DateTimeOffset.UtcNow;
        const string  authorName = "Test Author";
        const string  authorUrl = "https://example.com/author";
        const string  authorIconUrl = "https://example.com/author-icon.png";
        const string  footerText = "Test Footer";
        const string  footerIconUrl = "https://example.com/footer-icon.png";
        const string  imageUrl = "https://example.com/image.png";
        const string  thumbnailUrl = "https://example.com/thumbnail.png";
        const string  title = "Test Embed Title";
        const string  description = "This is a test embed description with all fields filled to demonstrate the complete functionality of the DiscordEmbedBuilder.";
        const string  url = "https://example.com/embed-url";
        const string  hexColor = "#FF6B6B";
        const string field1Name = "Field 1";
        const string field1Value = "This is the first field value";
        const string  field2Name = "Field 2";
        const string  field2Value = "This is the second field value";
        const string  field3Name = "Inline Field 1";
        const string  field3Value = "This is an inline field";
        const string  field4Name = "Inline Field 2";
        const string  field4Value = "This is another inline field";

        // Act
        var embed = new DiscordEmbedBuilder()
            .WithTitle(title)
            .WithDescription(description)
            .WithUrl(url)
            .WithTimestamp(timestamp)
            .WithColor(hexColor)
            .WithAuthor(authorName, authorUrl, authorIconUrl)
            .WithFooter(footerText, footerIconUrl)
            .WithImage(imageUrl)
            .WithThumbnail(thumbnailUrl)
            .AddField(field1Name, field1Value, false)
            .AddField(field2Name, field2Value, false)
            .AddField(field3Name, field3Value, true)
            .AddField(field4Name, field4Value, true)
            .Build();

        // Assert
        Assert.NotNull(embed);
        Assert.Equal(title, embed.Title);
        Assert.Equal(description, embed.Description);
        Assert.Equal(url, embed.Url);
        Assert.Equal(timestamp, embed.Timestamp);
        Assert.Equal(0xFF6B6B, embed.Color);
        
        // Author assertions
        Assert.NotNull(embed.Author);
        Assert.Equal(authorName, embed.Author.Name);
        Assert.Equal(authorUrl, embed.Author.Url);
        Assert.Equal(authorIconUrl, embed.Author.IconUrl);
        
        // Footer assertions
        Assert.NotNull(embed.Footer);
        Assert.Equal(footerText, embed.Footer.Text);
        Assert.Equal(footerIconUrl, embed.Footer.IconUrl);
        
        // Image assertions
        Assert.NotNull(embed.Image);
        Assert.Equal(imageUrl, embed.Image.Url);
        
        // Thumbnail assertions
        Assert.NotNull(embed.Thumbnail);
        Assert.Equal(thumbnailUrl, embed.Thumbnail.Url);
        
        // Fields assertions
        Assert.NotNull(embed.Fields);
        Assert.Equal(4, embed.Fields.Count);
        
        // Field 1 (non-inline)
        Assert.Equal(field1Name, embed.Fields[0].Name);
        Assert.Equal(field1Value, embed.Fields[0].Value);
        Assert.False(embed.Fields[0].Inline);
        
        // Field 2 (non-inline)
        Assert.Equal(field2Name, embed.Fields[1].Name);
        Assert.Equal(field2Value, embed.Fields[1].Value);
        Assert.False(embed.Fields[1].Inline);
        
        // Field 3 (inline)
        Assert.Equal(field3Name, embed.Fields[2].Name);
        Assert.Equal(field3Value, embed.Fields[2].Value);
        Assert.True(embed.Fields[2].Inline);
        
        // Field 4 (inline)
        Assert.Equal(field4Name, embed.Fields[3].Name);
        Assert.Equal(field4Value, embed.Fields[3].Value);
        Assert.True(embed.Fields[3].Inline);
    }

    [Fact]
    public void Build_WithPredefinedColor_ShouldSetCorrectColor()
    {
        // Arrange
        var expectedColor = (int)DiscordEmbedColor.Blue;

        // Act
        var embed = new DiscordEmbedBuilder()
            .WithTitle("Test Title")
            .WithColor(DiscordEmbedColor.Blue)
            .Build();

        // Assert
        Assert.Equal(expectedColor, embed.Color);
    }

    [Fact]
    public void Build_WithIntegerColor_ShouldSetCorrectColor()
    {
        // Arrange
        var expectedColor = 0xFF0000; // Red

        // Act
        var embed = new DiscordEmbedBuilder()
            .WithTitle("Test Title")
            .WithColor(expectedColor)
            .Build();

        // Assert
        Assert.Equal(expectedColor, embed.Color);
    }

    [Fact]
    public void Build_WithDateTimeTimestamp_ShouldConvertToDateTimeOffset()
    {
        // Arrange
        var dateTime = DateTime.UtcNow;

        // Act
        var embed = new DiscordEmbedBuilder()
            .WithTitle("Test Title")
            .WithTimestamp(dateTime)
            .Build();

        // Assert
        Assert.NotNull(embed.Timestamp);
        Assert.Equal(new DateTimeOffset(dateTime), embed.Timestamp);
    }

    [Fact]
    public void Build_WithMultipleFields_ShouldAddAllFields()
    {
        // Arrange
        var fieldCount = 10;

        // Act
        var builder = new DiscordEmbedBuilder().WithTitle("Test Title");
        
        for (int i = 0; i < fieldCount; i++)
        {
            builder.AddField($"Field {i}", $"Value {i}", i % 2 == 0);
        }
        
        var embed = builder.Build();

        // Assert
        Assert.NotNull(embed.Fields);
        Assert.Equal(fieldCount, embed.Fields.Count);
        
        for (int i = 0; i < fieldCount; i++)
        {
            Assert.Equal($"Field {i}", embed.Fields[i].Name);
            Assert.Equal($"Value {i}", embed.Fields[i].Value);
            Assert.Equal(i % 2 == 0, embed.Fields[i].Inline);
        }
    }

    [Fact]
    public void Build_WithMinimalFields_ShouldCreateValidEmbed()
    {
        // Act
        var embed = new DiscordEmbedBuilder()
            .WithTitle("Minimal Test")
            .Build();

        // Assert
        Assert.NotNull(embed);
        Assert.Equal("Minimal Test", embed.Title);
        Assert.Null(embed.Description);
        Assert.Null(embed.Url);
        Assert.Null(embed.Timestamp);
        Assert.Null(embed.Color);
        Assert.Null(embed.Author);
        Assert.Null(embed.Footer);
        Assert.Null(embed.Image);
        Assert.Null(embed.Thumbnail);
        Assert.Null(embed.Fields);
    }

    // Tests that exceed Discord limits
    [Fact]
    public void Build_WithTitleExceedingLimit_ShouldThrowDiscordValidationException()
    {
        var longTitle = new string('A', DiscordLimits.MaxTitleLength + 1);
        var exception = Assert.Throws<DiscordValidationException>(() =>
            new DiscordEmbedBuilder()
                .WithTitle(longTitle)
                .Build());
        Assert.Equal(DiscordValidationError.TitleTooLong, exception.ErrorCode);
    }

    [Fact]
    public void Build_WithDescriptionExceedingLimit_ShouldThrowDiscordValidationException()
    {
        var longDescription = new string('B', DiscordLimits.MaxDescriptionLength + 1);
        var exception = Assert.Throws<DiscordValidationException>(() =>
            new DiscordEmbedBuilder()
                .WithTitle("Test")
                .WithDescription(longDescription)
                .Build());
        Assert.Equal(DiscordValidationError.DescriptionTooLong, exception.ErrorCode);
    }

    [Fact]
    public void Build_WithAuthorNameExceedingLimit_ShouldThrowDiscordValidationException()
    {
        var longAuthorName = new string('C', DiscordLimits.MaxAuthorNameLength + 1);
        var exception = Assert.Throws<DiscordValidationException>(() =>
            new DiscordEmbedBuilder()
                .WithTitle("Test")
                .WithAuthor(longAuthorName)
                .Build());
        Assert.Equal(DiscordValidationError.AuthorNameTooLong, exception.ErrorCode);
    }

    [Fact]
    public void Build_WithFooterTextExceedingLimit_ShouldThrowDiscordValidationException()
    {
        var longFooterText = new string('D', DiscordLimits.MaxFooterTextLength + 1);
        var exception = Assert.Throws<DiscordValidationException>(() =>
            new DiscordEmbedBuilder()
                .WithTitle("Test")
                .WithFooter(longFooterText)
                .Build());
        Assert.Equal(DiscordValidationError.FooterTextTooLong, exception.ErrorCode);
    }

    [Fact]
    public void Build_WithTooManyFields_ShouldThrowDiscordValidationException()
    {
        var builder = new DiscordEmbedBuilder().WithTitle("Test");
        for (int i = 0; i < DiscordLimits.MaxFieldsPerEmbed + 1; i++)
        {
            builder.AddField($"Field {i}", $"Value {i}");
        }
        var exception = Assert.Throws<DiscordValidationException>(() => builder.Build());
        Assert.Equal(DiscordValidationError.TooManyFields, exception.ErrorCode);
    }

    [Fact]
    public void Build_WithFieldNameExceedingLimit_ShouldThrowDiscordValidationException()
    {
        var longFieldName = new string('E', DiscordLimits.MaxFieldNameLength + 1);
        var exception = Assert.Throws<DiscordValidationException>(() =>
            new DiscordEmbedBuilder()
                .WithTitle("Test")
                .AddField(longFieldName, "Value")
                .Build());
        Assert.Equal(DiscordValidationError.FieldNameTooLong, exception.ErrorCode);
    }

    [Fact]
    public void Build_WithFieldValueExceedingLimit_ShouldThrowDiscordValidationException()
    {
        var longFieldValue = new string('F', DiscordLimits.MaxFieldValueLength + 1);
        var exception = Assert.Throws<DiscordValidationException>(() =>
            new DiscordEmbedBuilder()
                .WithTitle("Test")
                .AddField("Field", longFieldValue)
                .Build());
        Assert.Equal(DiscordValidationError.FieldValueTooLong, exception.ErrorCode);
    }

    [Fact]
    public void Build_WithTotalContentExceedingLimit_ShouldThrowDiscordValidationException()
    {
        var longTitle = new string('G', DiscordLimits.MaxTitleLength);
        var longDescription = new string('H', DiscordLimits.MaxDescriptionLength);
        var longAuthorName = new string('I', DiscordLimits.MaxAuthorNameLength);
        var longFooterText = new string('J', DiscordLimits.MaxFooterTextLength);
        var exception = Assert.Throws<DiscordValidationException>(() =>
            new DiscordEmbedBuilder()
                .WithTitle(longTitle)
                .WithDescription(longDescription)
                .WithAuthor(longAuthorName)
                .WithFooter(longFooterText)
                .Build());
        Assert.Equal(DiscordValidationError.TotalContentTooLong, exception.ErrorCode);
    }

    [Fact]
    public void Build_WithInvalidHexColor_ShouldThrowDiscordValidationException()
    {
        var exception = Assert.Throws<DiscordValidationException>(() =>
            new DiscordEmbedBuilder()
                .WithTitle("Test")
                .WithColor("INVALID_COLOR")
                .Build());
        Assert.Equal(DiscordValidationError.InvalidHexColor, exception.ErrorCode);
    }

    [Fact]
    public void Build_WithNullHexColor_ShouldThrowDiscordValidationException()
    {
        var exception = Assert.Throws<DiscordValidationException>(() =>
            new DiscordEmbedBuilder()
                .WithTitle("Test")
                .WithColor(null!)
                .Build());
        Assert.Equal(DiscordValidationError.InvalidHexColor, exception.ErrorCode);
    }

    [Fact]
    public void Build_WithEmptyHexColor_ShouldThrowDiscordValidationException()
    {
        var exception = Assert.Throws<DiscordValidationException>(() =>
            new DiscordEmbedBuilder()
                .WithTitle("Test")
                .WithColor("")
                .Build());
        Assert.Equal(DiscordValidationError.InvalidHexColor, exception.ErrorCode);
    }

    [Fact]
    public void Build_WithInvalidUrl_ShouldThrowDiscordValidationException()
    {
        var exception = Assert.Throws<DiscordValidationException>(() =>
            new DiscordEmbedBuilder()
                .WithTitle("Test")
                .WithUrl("not-a-valid-url")
                .Build());
        Assert.Equal(DiscordValidationError.InvalidUrl, exception.ErrorCode);
    }

    [Fact]
    public void Build_WithInvalidAuthorUrl_ShouldThrowDiscordValidationException()
    {
        var exception = Assert.Throws<DiscordValidationException>(() =>
            new DiscordEmbedBuilder()
                .WithTitle("Test")
                .WithAuthor("Author", "invalid-url")
                .Build());
        Assert.Equal(DiscordValidationError.InvalidUrl, exception.ErrorCode);
    }

    [Fact]
    public void Build_WithInvalidImageUrl_ShouldThrowDiscordValidationException()
    {
        var exception = Assert.Throws<DiscordValidationException>(() =>
            new DiscordEmbedBuilder()
                .WithTitle("Test")
                .WithImage("invalid-image-url")
                .Build());
        Assert.Equal(DiscordValidationError.InvalidUrl, exception.ErrorCode);
    }

    [Fact]
    public void Build_WithEmptyFieldName_ShouldThrowDiscordValidationException()
    {
        var exception = Assert.Throws<DiscordValidationException>(() =>
            new DiscordEmbedBuilder()
                .WithTitle("Test")
                .AddField("", "Value")
                .Build());
        Assert.Equal(DiscordValidationError.EmptyFieldName, exception.ErrorCode);
    }

    [Fact]
    public void Build_WithEmptyFieldValue_ShouldThrowDiscordValidationException()
    {
        var exception = Assert.Throws<DiscordValidationException>(() =>
            new DiscordEmbedBuilder()
                .WithTitle("Test")
                .AddField("Field", "")
                .Build());
        Assert.Equal(DiscordValidationError.EmptyFieldValue, exception.ErrorCode);
    }
}