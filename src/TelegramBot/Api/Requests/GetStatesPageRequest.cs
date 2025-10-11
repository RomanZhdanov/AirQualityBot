using AirBro.TelegramBot.Helpers;
using AirBro.TelegramBot.Interfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace AirBro.TelegramBot.Api.Requests;

public record GetStatesPageRequest(long ChatId, int MessageId, string Country, int Page) : IApiRequest;

public class GetStatesPageRequestHandler : BaseApiRequestHandler<GetStatesPageRequest>
{
    public GetStatesPageRequestHandler(ILogger<BaseApiRequestHandler<GetStatesPageRequest>> logger, IAirApiService apiService, ITelegramBotClient botClient) : base(logger, apiService, botClient)
    {
    }

    protected override async Task HandleRequestAsync(GetStatesPageRequest request, CancellationToken cancellationToken)
    {
        var result = await ApiService.GetStatesPage(request.Country, request.Page, 10);

        Keyboard = MarkupHelper.GetStatesPage(request.Country, result);
        ResponseTextBuilder.AppendLine($"Country {request.Country} has been saved. Now select state for that country ({result.TotalCount} available, page {result.PageNumber}/{result.TotalPages})");
    }
}