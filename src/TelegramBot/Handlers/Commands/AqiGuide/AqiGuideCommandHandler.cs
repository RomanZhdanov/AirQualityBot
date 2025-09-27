using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AirBro.TelegramBot.Handlers.Commands.AqiGuide;

public class AqiGuideCommandHandler : IBotCommandHandler
{
    public async Task HandleAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var lines = new List<string>()
        {
            "0 - 50 Good\n\nAir quality is satisfactory and poses little or no risk. Ventilating your home is recommended.",
            "51 - 100 Moderate\n\nSensitive individuals should limit outdoor activity and go inside if experiencing respiratory symptoms such as coughing or shortness of breath.",
            "101 - 150 Unhealthy for Sensitive Groups\n\nSome members of the general public and all sensitive groups are at risk of health effects and should avoid outdoor activity if experiencing irritation or respiratory symptoms.",
            "151 - 200 Unhealthy\n\nIncreased likelihood of adverse effects and aggravation to the heart and lungs among general public - particularly for sensitive groups. Consider moving all activities inside.",
            "201 - 300 Very Unhealthy\n\nGeneral public will be noticeably affected. Sensitive groups will experience reduced endurance in activities. These individuals should remain indoors and restrict activities.",
            "Larger than 300 Hazardous\n\nGeneral public and sensitive groups are at high risk to experience strong irritations and adverse health effects that could trigger other illnesses. Everyone should avoid exercise and remain indoors."
        };
        var text = new StringBuilder();

        foreach (var entry in lines)
        {
            text.AppendLine(entry);
            text.AppendLine();
        }

        await botClient.SendMessage(
            chatId: chatId,
            text: text.ToString(),
            cancellationToken: cancellationToken);
    }
}