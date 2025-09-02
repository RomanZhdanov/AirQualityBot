using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Handlers;

public class MessageTextHandler
{

   public async Task HandleAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
   {
      var chatId =  message.Chat.Id;
      var messageText = message.Text;

      await botClient.SendMessage(
         chatId: chatId,
         text: messageText,
         cancellationToken: cancellationToken);
   }
}