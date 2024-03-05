using AirBro.TelegramBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Handlers;

public interface ICommandHandlers
{
    Task<Message> SetUserCountry(ITelegramBotClient botClient, long chatId, UserProfile userProfile, string country);

}