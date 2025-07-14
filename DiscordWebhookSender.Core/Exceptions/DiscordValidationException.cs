namespace DiscordWebhookSender.Core.Exceptions;

/// <summary>
/// Exception thrown when Discord webhook message validation fails.
/// This exception is thrown when message content exceeds Discord's limits or violates constraints.
/// </summary>
public class DiscordValidationException : Exception
{
    public DiscordValidationError ErrorCode { get; }

    public DiscordValidationException(DiscordValidationError errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }

    public DiscordValidationException(DiscordValidationError errorCode, string message, Exception innerException) : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
} 

public enum DiscordValidationError
{
    TitleTooLong,
    DescriptionTooLong,
    AuthorNameTooLong,
    FooterTextTooLong,
    FieldNameTooLong,
    FieldValueTooLong,
    TooManyFields,
    TotalContentTooLong,
    InvalidHexColor,
    InvalidUrl,
    EmptyFieldName,
    EmptyFieldValue,
    NullEmbed,
    NullField,
    NullOrEmptyWebhookMessage,
    ContentTooLong,
    UsernameTooLong,
    TooManyEmbeds,
}