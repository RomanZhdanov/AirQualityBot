using AirBro.TelegramBot.Data;
using AirBro.TelegramBot.Models;
using AirBro.TelegramBot.Models.Mappers;
using AirBro.TelegramBot.Services;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Handlers.Queries;

public class GetLocationAirQueryHandler : IBotQueryHandler
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ApiRequestsManagerService _apiRequestsManager;

    public GetLocationAirQueryHandler(ApplicationDbContext dbContext, ApiRequestsManagerService apiRequestsManager)
    {
        _dbContext = dbContext;
        _apiRequestsManager = apiRequestsManager;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var chatId = callbackQuery.Message!.Chat.Id;
        var messageid = callbackQuery.Message.MessageId;
        var args = callbackQuery.Data!.Split("|");
        var locationId = Convert.ToInt32(args[1]);
        
        var location = await _dbContext.Locations
            .Include(l => l.Country)
            .SingleOrDefaultAsync(l => l.Id == locationId, cancellationToken);

        if (location is null)
        {
            await botClient.EditMessageText(
                chatId: chatId,
                messageId: messageid,
                text: "Location not found",
                cancellationToken: cancellationToken);

            return;
        }

        await botClient.EditMessageText(
            chatId: chatId,
            messageId: messageid,
            text: "Fetching data please wait...",
            cancellationToken: cancellationToken);

        await _apiRequestsManager.DispatchGetLocationRequestAsync(chatId, messageid, false, location.ToLocationDto());
    }
}