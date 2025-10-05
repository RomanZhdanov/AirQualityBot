namespace AirBro.TelegramBot.Models;

public class LocationDto
{
    public string? City { get; set; }

    public string? State { get; set; }

    public string? Country { get; set; }

    public double? Longitude { get; set; }
    
    public double? Latitude { get; set; }

    public LocationDto()
    {
        City = null;
        State = null;
        Country = null;
        Longitude = null;
        Latitude = null;
    }

    public LocationDto(string city, string state, string country)
    {
        City = city;
        State = state;
        Country = country;
        Longitude = null;
        Latitude = null;
    }

    public LocationDto(string city, string state, string country, double[] coordinates)
    {
        City = city;
        State = state;
        Country = country;

        if (coordinates.Length == 2)
        {
            Longitude = coordinates[0];
            Latitude = coordinates[1];
        }
    }

    public LocationDto(string city, string state, string country, double? longitude, double? latitude)
    {
        City = city;
        State = state;
        Country = country;
        Longitude = longitude;
        Latitude = latitude;
    }

    public LocationDto(double longitude, double latitude)
    {
        Longitude = longitude;
        Latitude = latitude;
        City = null;
        State = null;
        Country = null;
    }

    public override string ToString()
    {
        return $"{City} ({Country})";
    }
}