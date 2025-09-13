using AirBro.TelegramBot.Exceptions;
using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace AirBro.TelegramBot.Handlers.Queries;

public class RemoveLocationQueryHandler : IBotQueryHandler
{
    private readonly UserDataService _userDataService;

    public RemoveLocationQueryHandler(UserDataService userDataService)
    {
        _userDataService = userDataService;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var chatId = callbackQuery.Message!.Chat.Id;
        var messageId = callbackQuery.Message.MessageId;
        var args = callbackQuery.Data!.Split('|');
        var locationId = Convert.ToInt32(args[1]);
        
        var buttons = new List<InlineKeyboardButton>
        {
            InlineKeyboardButton.WithCallbackData("Back to my locations", $"GetUserLocations")
        };
        var keyboard = new InlineKeyboardMarkup(buttons);

        try
        {
            var location = await _userDataService
                .RemoveUserLocationAsync(chatId, locationId);

            await botClient.EditMessageText(
                chatId: chatId,
                messageId: messageId,
                text: $"Location {location} was successfully removed from your list.",
                replyMarkup: keyboard,
                cancellationToken: cancellationToken);
        }
        catch (LocationNotFoundException e)
        {
            await botClient.EditMessageText(
                chatId: chatId,
                messageId: messageId,
                text: "Location with this id was not found.",
                replyMarkup: keyboard,
                cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            await botClient.EditMessageText(
                chatId: chatId,
                messageId: messageId,
                text: $"Error while removing location: {e.Message}",
                replyMarkup: keyboard,
                cancellationToken: cancellationToken);
        }
    }
}