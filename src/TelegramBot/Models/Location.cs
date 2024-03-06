namespace AirBro.TelegramBot.Models;

public class Location
{
    public string City { get; }

    public string State { get; }

    public string Country { get; }

    public Location(string city, string state, string country)
    {
        City = city;
        State = state;
        Country = country;
    }

    public override string ToString()
    {
        return $"{City} ({Country})";
    }
}