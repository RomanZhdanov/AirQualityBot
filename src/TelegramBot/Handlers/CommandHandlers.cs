using AirBro.TelegramBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Handlers;

public class CommandHandlers : ICommandHandlers
{
    public async Task<Message> SetUserCountry(ITelegramBotClient botClient, long chatId, UserProfile userProfile, string country)
    {
        userProfile.Country = country;
        
        return await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: $"Country {country} successfully saved.");
    }
}