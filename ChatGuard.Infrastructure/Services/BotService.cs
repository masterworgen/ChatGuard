using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace ChatGuard.Infrastructure.Services;

public class BotService(ApplicationContext context, ILogger<BotService> logger, ITelegramBotClient botClient)
{
    public async Task<bool> RemoveUserFromChatsAsync(string username, CancellationToken token)
    {
        var chats = await context.Chats.ToListAsync(token);

        var result = true;

        foreach (var chat in chats)
        {
            try
            {
                var telegramChat = new ChatId(chat.Id);
                var chatMembers = await ((WTelegramBotClient)botClient).GetChatMemberList(telegramChat);

                var userChat = chatMembers.FirstOrDefault(x => x.User.Username == username);

                if (userChat is null)
                    throw new MissingUserException();

                await botClient.BanChatMemberAsync(telegramChat, userChat!.User.Id, DateTime.Now.AddSeconds(1),
                    cancellationToken: token);

                logger.LogInformation("Пользователь {0} удален из чата {1}.", username, chat);
            }
            catch (ApiRequestException ex)
            {
                result = false;
                logger.LogError(ex, "Ошибка при удалении пользователя {0} из чата {1}", username, chat);
            }
            catch (MissingUserException ex)
            {
                result = false;
                logger.LogError(ex, "Не найден пользователь с ником {0}", username);
                break;
            }
            catch (Exception e)
            {
                context.Chats.Remove(chat);
                await context.SaveChangesAsync(token);
                logger.LogError(e, "Глобальная ошибка");
            }
        }

        return result;
    }
}

file class MissingUserException : Exception;