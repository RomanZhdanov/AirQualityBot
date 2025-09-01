using AirBro.TelegramBot.Handlers.Commands.AddUserLocation;
using AirBro.TelegramBot.Handlers.Commands.ShowAir;
using AirBro.TelegramBot.Handlers.Commands.ShowUserLocations;
using AirBro.TelegramBot.Handlers.Commands.Welcome;
using AirBro.TelegramBot.Handlers.Queries;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Handlers;

public class UpdateHandlers : IUpdateHandlers
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Type> _commands;
    private readonly Dictionary<string, Type> _queries;
    
    public UpdateHandlers(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _commands = new Dictionary<string, Type>
        {
            { "/start", typeof(WelcomeCommandHandler) },
            { "/add_location", typeof(AddUserLocationCommandHandler) },
            { "/my_locations", typeof(ShowUserLocationsCommandHandler) },
            { "/show_air", typeof(ShowAirCommandHandler) }
        };

        _queries = new Dictionary<string, Type>
        {
            { "SetCountry", typeof(SetCountryQueryHandler) },
            { "SetState", typeof(SetStateQueryHandler) },
            { "SetCity", typeof(SetCityQueryHandler) },
            { "CountriesPage", typeof(CountriesPageQueryHandler)},
            { "StatesPage", typeof(StatesPageQueryHandler)},
            { "CitiesPage", typeof(CitiesPageQueryHandler) }
        };
    }
    
    public async Task HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var input = message.Text?.Split(' ')[0];

        if (input != null)
        {
            if (_commands.TryGetValue(input, out var commandType))
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var handler = scope.ServiceProvider.GetRequiredService(commandType) as IBotCommandHandler ?? throw new InvalidOperationException();
                    await handler.HandleAsync(botClient, message, cancellationToken);
                }
                catch (Exception e)
                {
                    await botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: $"There was an error: {e.Message}",
                        cancellationToken: cancellationToken);
                }
            }
            else
            {
                await botClient.SendMessage(
                    chatId:  message.Chat.Id,
                    text: $"Can't find handler for this command",
                    cancellationToken: cancellationToken);
            }
        }
    }

    public async Task HndleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery query, CancellationToken cancellationToken)
    {
        if (query.Message is null || query.Data is null)
        {
            return;
        }
        
        var data = query.Data!;
        var key = data.Split('|')[0];

        if (_queries.TryGetValue(key, out var queryType))
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService(queryType) as IBotQueryHandler ?? throw new InvalidOperationException();
                await handler.HandleAsync(botClient, query, cancellationToken);
            }
            catch (Exception e)
            {
                await botClient.SendMessage(
                    chatId: query.Message.Chat.Id,
                    text: $"There was an error: {e.Message}",
                    cancellationToken: cancellationToken);
            }
        }
        else
        {
            await botClient.AnswerCallbackQuery(
                callbackQueryId: query.Id,
                text: "Can't find handler for this query",
                cancellationToken: cancellationToken);
        }
    }

    public Task HandleUnknownAsync(ITelegramBotClient botClient, Update update)
    {
        Console.WriteLine($"Unknown update type: {update.Type}");
        return Task.CompletedTask;
    }
}