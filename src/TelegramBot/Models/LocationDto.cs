namespace AirBro.TelegramBot.Models;

public class LocationDto
{
    public string? City { get; set; }

    public string? State { get; set; }

    public string? Country { get; set; }

    public LocationDto()
    {
        City = null;
        State = null;
        Country = null;
    }

    public LocationDto(string city, string state, string country)
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