namespace AirBro.TelegramBot.Models;

public class UserProfile
{
    public string Country { get; set; }

    public string State { get; set; }

    public string City { get; set; }

    public ConversationFlow Flow { get; set; } = new();
}