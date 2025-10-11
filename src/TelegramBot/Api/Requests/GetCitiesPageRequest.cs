using AirBro.TelegramBot.Helpers;
using AirBro.TelegramBot.Interfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace AirBro.TelegramBot.Api.Requests;

public record GetCitiesPageRequest(long ChatId, int MessageId, string Country, string State, int Page) : IApiRequest;

public class GetCitiesPageRequestHandler : BaseApiRequestHandler<GetCitiesPageRequest>
{
    public GetCitiesPageRequestHandler(ILogger<BaseApiRequestHandler<GetCitiesPageRequest>> logger, IAirApiService apiService, ITelegramBotClient botClient) : base(logger, apiService, botClient)
    {
    }

    protected override async Task HandleRequestAsync(GetCitiesPageRequest request, CancellationToken cancellationToken)
    {
        var result = await ApiService.GetCitiesPage(request.Country, request.State, request.Page, 10);

        if (result is null)
        {
            Keyboard.AddButton(
                InlineKeyboardButton.WithCallbackData("Back to states list", $"StatesPage|{request.Country}|1"));
            ResponseTextBuilder.AppendLine($"Can't fetch data for location  {request.State}, we're sorry.");
            return;
        }

        Keyboard = MarkupHelper.GetCitiesPage(request.Country, request.State, result);
        
        ResponseTextBuilder.AppendLine(
            $"State {request.State} has been saved. Finally select city ({result.TotalCount} available, page {result.PageNumber}/{result.TotalPages})");
    }
}