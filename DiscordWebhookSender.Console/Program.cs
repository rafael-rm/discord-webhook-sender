using DiscordWebhookSender.Core;

namespace DiscordWebhookSender.Console;

internal abstract class DiscordProgram
{
    static async Task Main()
    {
        var embed = new DiscordEmbedBuilder()
            .WithTitle("🚀 Deploy Completed Successfully!")
            .WithDescription("The new application version has been published and is operating normally.")
            .WithUrl("https://suaempresa.com/deploys/2.1.0")
            .WithTimestamp(DateTimeOffset.UtcNow)
            .WithColor(0x1ABC9C)
            .WithAuthor("Hyzen CI/CD", "https://suaempresa.com", "https://cdn-icons-png.flaticon.com/512/5968/5968756.png")
            .WithThumbnail("https://cdn-icons-png.flaticon.com/512/190/190411.png")
            .WithImage("https://suaempresa.com/assets/deploy-image.png")
            .AddField("Version", "2.1.0", true)
            .AddField("Environment", "Production", true)
            .AddField("Duration", "12 minutes", true)
            .AddField("Started by", "Rafael Rodrigues", true)
            .AddField("Notes", "No errors found during deployment.")
            .WithFooter("CI Pipeline • 2025 © Hyzen", "https://cdn-icons-png.flaticon.com/512/25/25231.png")
            .Build();
        
        var message = new DiscordWebhookMessageBuilder()
            .WithAvatarUrl("https://cdn-icons-png.flaticon.com/512/5968/5968756.png")
            .WithUsername("Hyzen CI/CD Bot")
            .WithContent("✅ Automatic update completed")
            .AddEmbed(embed)
            .Build();

        var client = DiscordWebhookClient.Get();
        await client.SendAsync("YOUR_DISCORD_WEBHOOK_URL", message);
        await client.SendAsync("YOUR_DISCORD_WEBHOOK_URL", embed);
        await client.SendAsync("YOUR_DISCORD_WEBHOOK_URL", "This is a simple text message");
    }
}