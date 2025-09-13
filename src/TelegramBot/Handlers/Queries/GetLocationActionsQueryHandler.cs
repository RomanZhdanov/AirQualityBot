using AirBro.TelegramBot.Data;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace AirBro.TelegramBot.Handlers.Queries;

public class GetLocationActionsQueryHandler : IBotQueryHandler
{
    private readonly ApplicationDbContext _dbContext;

    public GetLocationActionsQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var chatId = callbackQuery.Message!.Chat.Id;
        var messageId = callbackQuery.Message.MessageId;
        var args = callbackQuery.Data!.Split("|");
        var locationId = Convert.ToInt32(args[1]);
        var location = _dbContext.Locations
            .Include(l => l.Country)
            .FirstOrDefault(x => x.Id == locationId);

        if (location is null)
        {
            await botClient.EditMessageText(
                chatId: chatId,
                messageId: messageId,
                text: "There is no location with that id.",
                cancellationToken: cancellationToken);

            return;
        }
        
        var buttons = new List<List<InlineKeyboardButton>>
        {
            new() 
            {
                InlineKeyboardButton.WithCallbackData("Check air", $"GetLocationAir|{location.Id}")
            },
            new()
            {
                InlineKeyboardButton.WithCallbackData("Remove", $"RemoveLocation|{location.Id}")
            },
            new()
            {
                InlineKeyboardButton.WithCallbackData("Back to my locations", $"GetUserLocations")
            }
        };
            
        var keyboard = new InlineKeyboardMarkup(buttons);
        
        await botClient.EditMessageText(
            chatId: chatId,
            messageId: messageId,
            text: $"Choose action for location {location}",
            replyMarkup: keyboard,
            cancellationToken: cancellationToken);
    }
}