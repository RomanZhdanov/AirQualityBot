using AirBro.TelegramBot.Models;
using IQAirApiClient.Models;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace AirBro.TelegramBot.Helpers;

public class MessagesHelper
{
    public static async Task SendStatesPageMessage(ITelegramBotClient botClient,
        string country, PaginatedList<StateItem> statesPage, long chatId, int messageId, CancellationToken cancellationToken)
    {
        var keyboard = MarkupHelper.GetStatesPage(country, statesPage);

        await botClient.EditMessageText(
            chatId: chatId,
            messageId: messageId,
            text: $"Country {country} has been saved. Now select state for that country ({statesPage.TotalCount} available, page {statesPage.PageNumber}/{statesPage.TotalPages})",
            replyMarkup: keyboard,
            cancellationToken: cancellationToken);
    }
    
    public static async Task SendCitiesPageMessage(ITelegramBotClient botClient,
        PaginatedList<CityItem>? citiesPage, string country, long chatId, int messageId, string state, CancellationToken cancellationToken)
    {
        if (citiesPage is null)
        {
            var buttons = new List<InlineKeyboardButton>();
            buttons.Add(InlineKeyboardButton.WithCallbackData("Back to states list", $"StatesPage|{country}|1"));
            
            await botClient.EditMessageText(
                chatId:  chatId,
                messageId: messageId,
                text: $"API can't find cities for state {state}, we're sorry.",
                replyMarkup: new InlineKeyboardMarkup(buttons),
                cancellationToken: cancellationToken);
            return;
        }
        
        var keyboard = MarkupHelper.GetCitiesPage(country, state, citiesPage);
            
        await botClient.EditMessageText(
            chatId: chatId,
            messageId: messageId,
            text: $"State {state} has been saved. Finally select city ({citiesPage.TotalCount} available, page {citiesPage.PageNumber}/{citiesPage.TotalPages})",
            replyMarkup: keyboard,
            cancellationToken: cancellationToken);
    }}