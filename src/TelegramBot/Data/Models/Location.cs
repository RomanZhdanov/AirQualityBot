namespace AirBro.TelegramBot.Data.Models;

public class Location
{
    public int Id { get; set; }

    public string Country { get; set; }

    public string State { get; set; }

    public string City { get; set; }
    
    public List<User> Users { get; } = [];
}