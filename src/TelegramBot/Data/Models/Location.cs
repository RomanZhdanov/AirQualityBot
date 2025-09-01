namespace AirBro.TelegramBot.Data.Models;

public class Location
{
    public int Id { get; set; }

    public int CountryId { get; set; }

    public string State { get; set; }

    public string? City { get; set; }

    public bool Healthy { get; set; }
    
    public List<User> Users { get; } = [];
    
    public Country? Country { get; set; }
    
    public override string ToString()
    {
        return $"{City} ({Country?.Name})";
    }
}