using DiscordWebhookSender;
using DiscordWebhookSender.Models;

namespace ConsoleApp;

internal abstract class Program
{
    static async Task Main()
    {
        var embed = new DiscordEmbedBuilder()
            .WithTitle("🚀 Deploy Concluído com Sucesso!")
            .WithDescription("A nova versão da aplicação foi publicada e está operando normalmente.")
            .WithUrl("https://suaempresa.com/deploys/2.1.0")
            .WithTimestamp(DateTimeOffset.UtcNow)
            .WithColor(0x1ABC9C)
            .WithAuthor("Hyzen CI/CD", "https://suaempresa.com", "https://cdn-icons-png.flaticon.com/512/5968/5968756.png")
            .WithThumbnail("https://cdn-icons-png.flaticon.com/512/190/190411.png")
            .WithImage("https://suaempresa.com/assets/deploy-image.png")
            .AddField("Versão", "2.1.0", true)
            .AddField("Ambiente", "Produção", true)
            .AddField("Duração", "12 minutos", true)
            .AddField("Iniciado por", "João Silva", true)
            .AddField("Notas", "Nenhum erro encontrado durante o deploy.", false)
            .WithFooter("CI Pipeline • 2025 © Hyzen", "https://cdn-icons-png.flaticon.com/512/25/25231.png")
            .Build();

        var message = new DiscordWebhookMessage
        {
            AvatarUrl = "https://cdn-icons-png.flaticon.com/512/5968/5968756.png",
            Username = "Hyzen CI/CD Bot",
            Content = "✅ Atualização automática concluída",
            Embeds = [embed]
        };

        var client = new DiscordWebhookClient();
        await client.SendAsync("discord.webhook", message);
    }
}