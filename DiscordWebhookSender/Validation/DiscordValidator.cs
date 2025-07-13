using DiscordWebhookSender.Exceptions;
using DiscordWebhookSender.Models;

namespace DiscordWebhookSender.Validation;

/// <summary>
/// Validates Discord webhook messages and embeds against Discord's API limits and constraints.
/// </summary>
public static class DiscordValidator
{
    /// <summary>
    /// Validates a Discord webhook message against Discord's limits.
    /// </summary>
    /// <param name="message">The message to validate.</param>
    /// <exception cref="DiscordValidationException">Thrown when validation fails.</exception>
    public static void ValidateWebhookMessage(DiscordWebhookMessage message)
    {
        if (message is null)
            throw new DiscordValidationException("Webhook message cannot be null.");

        if (!string.IsNullOrEmpty(message.Content) && message.Content.Length > DiscordLimits.MaxContentLength)
        {
            throw new DiscordValidationException(
                $"Message content exceeds the maximum length of {DiscordLimits.MaxContentLength} characters. " +
                $"Current length: {message.Content.Length} characters.");
        }

        if (!string.IsNullOrEmpty(message.Username) && message.Username.Length > DiscordLimits.MaxUsernameLength)
        {
            throw new DiscordValidationException(
                $"Username exceeds the maximum length of {DiscordLimits.MaxUsernameLength} characters. " +
                $"Current length: {message.Username.Length} characters.");
        }

        if (message.Embeds != null)
        {
            if (message.Embeds.Count > DiscordLimits.MaxEmbedsPerMessage)
            {
                throw new DiscordValidationException(
                    $"Message contains too many embeds. Maximum allowed: {DiscordLimits.MaxEmbedsPerMessage}. " +
                    $"Current count: {message.Embeds.Count}.");
            }

            foreach (var embed in message.Embeds)
            {
                ValidateEmbed(embed);
            }
        }
    }

    /// <summary>
    /// Validates a Discord embed against Discord's limits.
    /// </summary>
    /// <param name="embed">The embed to validate.</param>
    /// <exception cref="DiscordValidationException">Thrown when validation fails.</exception>
    public static void ValidateEmbed(DiscordEmbed embed)
    {
        if (embed is null)
            throw new DiscordValidationException("Embed cannot be null.");

        var totalLength = 0;

        if (!string.IsNullOrEmpty(embed.Title))
        {
            if (embed.Title.Length > DiscordLimits.MaxTitleLength)
            {
                throw new DiscordValidationException(
                    $"Embed title exceeds the maximum length of {DiscordLimits.MaxTitleLength} characters. " +
                    $"Current length: {embed.Title.Length} characters.");
            }
            totalLength += embed.Title.Length;
        }

        if (!string.IsNullOrEmpty(embed.Description))
        {
            if (embed.Description.Length > DiscordLimits.MaxDescriptionLength)
            {
                throw new DiscordValidationException(
                    $"Embed description exceeds the maximum length of {DiscordLimits.MaxDescriptionLength} characters. " +
                    $"Current length: {embed.Description.Length} characters.");
            }
            totalLength += embed.Description.Length;
        }

        if (embed.Author != null)
        {
            if (!string.IsNullOrEmpty(embed.Author.Name))
            {
                if (embed.Author.Name.Length > DiscordLimits.MaxAuthorNameLength)
                {
                    throw new DiscordValidationException(
                        $"Author name exceeds the maximum length of {DiscordLimits.MaxAuthorNameLength} characters. " +
                        $"Current length: {embed.Author.Name.Length} characters.");
                }
                totalLength += embed.Author.Name.Length;
            }
        }

        if (embed.Footer != null && !string.IsNullOrEmpty(embed.Footer.Text))
        {
            if (embed.Footer.Text.Length > DiscordLimits.MaxFooterTextLength)
            {
                throw new DiscordValidationException(
                    $"Footer text exceeds the maximum length of {DiscordLimits.MaxFooterTextLength} characters. " +
                    $"Current length: {embed.Footer.Text.Length} characters.");
            }
            totalLength += embed.Footer.Text.Length;
        }

        if (embed.Fields != null)
        {
            if (embed.Fields.Count > DiscordLimits.MaxFieldsPerEmbed)
            {
                throw new DiscordValidationException(
                    $"Embed contains too many fields. Maximum allowed: {DiscordLimits.MaxFieldsPerEmbed}. " +
                    $"Current count: {embed.Fields.Count}.");
            }

            foreach (var field in embed.Fields)
            {
                ValidateField(field, ref totalLength);
            }
        }

        if (totalLength > DiscordLimits.MaxTotalEmbedLength)
        {
            throw new DiscordValidationException(
                $"Total embed content length exceeds the maximum of {DiscordLimits.MaxTotalEmbedLength} characters. " +
                $"Current total length: {totalLength} characters.");
        }
    }

    /// <summary>
    /// Validates a Discord embed field against Discord's limits.
    /// </summary>
    /// <param name="field">The field to validate.</param>
    /// <param name="totalLength">Reference to the total length counter.</param>
    /// <exception cref="DiscordValidationException">Thrown when validation fails.</exception>
    private static void ValidateField(DiscordEmbedField field, ref int totalLength)
    {
        if (field is null)
            throw new DiscordValidationException("Embed field cannot be null.");

        if (string.IsNullOrEmpty(field.Name))
        {
            throw new DiscordValidationException("Field name cannot be null or empty.");
        }

        if (field.Name.Length > DiscordLimits.MaxFieldNameLength)
        {
            throw new DiscordValidationException(
                $"Field name exceeds the maximum length of {DiscordLimits.MaxFieldNameLength} characters. " +
                $"Current length: {field.Name.Length} characters.");
        }
        totalLength += field.Name.Length;

        if (string.IsNullOrEmpty(field.Value))
        {
            throw new DiscordValidationException("Field value cannot be null or empty.");
        }

        if (field.Value.Length > DiscordLimits.MaxFieldValueLength)
        {
            throw new DiscordValidationException(
                $"Field value exceeds the maximum length of {DiscordLimits.MaxFieldValueLength} characters. " +
                $"Current length: {field.Value.Length} characters.");
        }
        totalLength += field.Value.Length;
    }
    
    /// <summary>
    /// Validates the webhook URL.
    /// </summary>
    /// <param name="webhookUrl">The webhook URL to validate.</param>
    /// <exception cref="DiscordValidationException">Thrown when the webhook URL is null, empty, or whitespace.</exception>
    public static void ValidateWebhookUrl(string webhookUrl)
    {
        if (string.IsNullOrWhiteSpace(webhookUrl))
            throw new DiscordValidationException("Webhook URL cannot be null, empty, or whitespace.");
    }
} 