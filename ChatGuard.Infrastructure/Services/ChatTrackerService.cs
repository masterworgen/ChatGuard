using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Chat = ChatGuard.Domain.Entity.Chat;

namespace ChatGuard.Infrastructure.Services;

public class ChatTrackerService(IServiceScopeFactory serviceScopeFactory) : IUpdateHandler
{
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        await using var serviceScope = serviceScopeFactory.CreateAsyncScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationContext>();
        var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<ChatTrackerService>>();

        if (update is not { Type: UpdateType.Message, Message.Chat.Type: ChatType.Group or ChatType.Supergroup })
        {
            return;
        }

        var chatId = update.Message.Chat.Id;

        var chatExist = await context.Chats.AnyAsync(x => x.Id == chatId, cancellationToken);

        if (!chatExist)
        {
            await context.Chats.AddAsync(new Chat { Id = chatId }, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Добавлен чат {0}", chatId);
        }
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        await using var serviceScope = serviceScopeFactory.CreateAsyncScope();
        var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<ChatTrackerService>>();

        logger.LogError(exception, "Произшла глобальная ошибка");
    }
}