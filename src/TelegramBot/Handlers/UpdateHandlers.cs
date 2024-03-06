using System.Text;
using AirBro.TelegramBot.Commands;
using AirBro.TelegramBot.Helpers;
using AirBro.TelegramBot.Models;
using AirBro.TelegramBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Message = Telegram.Bot.Types.Message;

namespace AirBro.TelegramBot.Handlers;

public class UpdateHandlers : IUpdateHandlers
{
    private readonly IQAirService _airService;
    private readonly Dictionary<long, UserProfile> _usersData;
    private readonly CommandsManager _commandsManager;

    public UpdateHandlers(IQAirService airService, Dictionary<long, UserProfile> usersData)
    {
        _airService = airService;
        _usersData = usersData;
        _commandsManager = new CommandsManager(_airService, _usersData);
    }
    
    public async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
        
        Message respMsg = null;
        var chatId = message.Chat.Id;
        UserProfile userProfile;

        if (_usersData.ContainsKey(chatId))
        {
            userProfile = _usersData[chatId];
        }
        else
        {
            userProfile = new UserProfile();
            _usersData.Add(chatId, userProfile);
        }

        var action = message.Text!.Split(' ')[0] switch
        {
            "/set_location" => SetUserLocationDialog(botClient, cancellationToken),
            "/show_air" => SendAirQuality(botClient, message, userProfile, cancellationToken),
            _ => SendStartMessage(botClient, message)
        };

        respMsg = await action;
        
        async Task<Message> SendStartMessage(ITelegramBotClient botClient, Message message)
        {
            var msg = new StringBuilder();
            msg.AppendLine($"Бот представляет из себя дополнение к приложению WireGuard\\.");

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: msg.ToString(),
                parseMode: ParseMode.MarkdownV2,
                replyMarkup: new ReplyKeyboardRemove());
        }

        async Task<Message> SendAirQuality(ITelegramBotClient bot, Message message, UserProfile userProfile,
            CancellationToken cancellationToken)
        {
            var result = await _airService.GetAirForCity(userProfile.City, userProfile.State, userProfile.Country);
            
            var msgText = new StringBuilder();
            msgText.AppendLine($"Pollution in {result.City}:");
            msgText.AppendLine($"AQI US: {result.Current.Pollution.Aqius}");
            msgText.AppendLine($"AQI China: {result.Current.Pollution.Aqicn}");
            msgText.AppendLine($"main pollutant for US AQI: {result.Current.Pollution.Mainus}");
            msgText.AppendLine($"main pollutant for Chinese AQI: {result.Current.Pollution.Maincn}");
            msgText.AppendLine(
                $"Date: {result.Current.Pollution.Ts.ToShortDateString()} Time: {result.Current.Pollution.Ts.ToShortTimeString()}");
            
            return await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: msgText.ToString(),
                cancellationToken: cancellationToken);
        }

        async Task<Message> SetUserLocationDialog(ITelegramBotClient bot, CancellationToken cancellationToken)
        {
            string msg = string.Empty;
            IReplyMarkup keyboard = new ReplyKeyboardRemove();
            
            var countriesPage = _airService.GetCountriesPage(1, 10);
            msg = $"Lets start from choosing a country. Select country ({countriesPage.TotalCount} available, page {countriesPage.PageNumber}/{countriesPage.TotalPages})";
            keyboard = InlineKeyboardHelper.GetCountriesPage(countriesPage);
            
            return await bot.SendTextMessageAsync(
                chatId: chatId,
                text: msg,
                replyMarkup: keyboard,
                cancellationToken: cancellationToken);
        }
    }

    public async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery query, CancellationToken cancellationToken)
    {
        if (query.Message is null || query.Data is null)
        {
            return;
        }

        string[] args = query.Data.Split('|');

        if (args.Length > 0)
        {
            var command = args[0];
            
            await botClient.AnswerCallbackQueryAsync(
                callbackQueryId: query.Id, 
                cancellationToken: cancellationToken);

            var handler = _commandsManager.GetCommandHandler(command);
            await handler.HandleAsync(botClient, query.Message, args, cancellationToken);
        }
    }

    public Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
    {
        Console.WriteLine($"Unknown update type: {update.Type}");
        return Task.CompletedTask;
    }
}