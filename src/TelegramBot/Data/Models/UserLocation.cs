using System.ComponentModel.DataAnnotations;

namespace AirBro.TelegramBot.Data.Models;

public class UserLocation
{
    [Key]
    public long ChatId { get; private set; }

    public string? Country { get; set; }

    public string? State { get; set; }

    public string? City { get; set; }

    public UserLocation(long chatId)
    {
        ChatId = chatId;
    }
}