using AirBro.TelegramBot.Data;
using AirBro.TelegramBot.Models;
using AirBro.TelegramBot.Services;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace AirBro.TelegramBot.Handlers;

public class MessageTextHandler
{
   private readonly TempUserDataService  _tempUserDataService;
   private readonly ApplicationDbContext _dbContext;

   public MessageTextHandler(TempUserDataService tempUserDataService,  ApplicationDbContext dbContext)
   {
      _tempUserDataService = tempUserDataService;
      _dbContext = dbContext;
   }

   public async Task HandleAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
   {
      var chatId =  message.Chat.Id;
      var input = message.Text;

      switch (_tempUserDataService.State)
      {
         case UserStates.CountrySearch:
            var result = await _dbContext.Countries
               .Where(c => c.Name.ToLower().Contains(input.ToLower()))
               .ToListAsync(cancellationToken);
            if (result.Any())
            {
               var buttons = new List<InlineKeyboardButton>();

               foreach (var country in result)
               {
                  buttons.Add(InlineKeyboardButton.WithCallbackData(country.Name, $"SetCountry|{country.Name}"));
               }
               var keyboard = new InlineKeyboardMarkup(buttons);
               await botClient.SendMessage(
                  chatId: chatId,
                  text: $"This is what I've found for {input}, you can select country or send me another one",
                  replyMarkup: keyboard,
                  cancellationToken: cancellationToken);
            }
            else
            {
               await botClient.SendMessage(
                  chatId: chatId,
                  text: $"Can't find anything for {input}",
                  cancellationToken: cancellationToken);  
            }
            break;
         default:
            await botClient.SendMessage(
               chatId: chatId,
               text: input,
               cancellationToken: cancellationToken);
            break;
      }
   }
}