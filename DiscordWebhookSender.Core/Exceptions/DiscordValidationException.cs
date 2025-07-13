namespace DiscordWebhookSender.Core.Exceptions;

/// <summary>
/// Exception thrown when Discord webhook message validation fails.
/// This exception is thrown when message content exceeds Discord's limits or violates constraints.
/// </summary>
public class DiscordValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the DiscordValidationException class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public DiscordValidationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the DiscordValidationException class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DiscordValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }
} 