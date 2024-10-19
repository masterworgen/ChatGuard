using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace ChatGuard.Infrastructure.Background;

public class BotBackground(ITelegramBotClient botClient, IServiceScopeFactory serviceScope) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var service = serviceScope.CreateAsyncScope();
        var botService = service.ServiceProvider.GetRequiredService<IUpdateHandler>();
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { } // receive all update types
        };

        botClient.StartReceiving(botService.HandleUpdateAsync, botService.HandleErrorAsync, receiverOptions, stoppingToken);
    }
}