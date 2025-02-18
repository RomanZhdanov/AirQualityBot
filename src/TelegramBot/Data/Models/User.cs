namespace AirBro.TelegramBot.Data.Models;

public class User
{
    public long Id { get; set; }

    public List<Location> Locations { get; } = [];
}