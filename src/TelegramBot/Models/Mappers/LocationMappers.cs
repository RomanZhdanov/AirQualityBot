namespace AirBro.TelegramBot.Models.Mappers;

public static class LocationMappers
{
    public static TelegramBot.Models.Location ToLocation(this TelegramBot.Data.Models.Location location)
    {
        return new Location(location.City, location.State, location.Country.Name);
    }
}