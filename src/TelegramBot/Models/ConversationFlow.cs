namespace AirBro.TelegramBot.Models;

public class ConversationFlow
{
    public enum Question
    {
        None,
        Country,
        State,
        City
    }

    public Question LastQuestionAsked { get; set; } = Question.None;
}