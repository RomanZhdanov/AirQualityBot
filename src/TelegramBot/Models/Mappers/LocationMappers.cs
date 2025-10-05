namespace AirBro.TelegramBot.Models.Mappers;

public static class LocationMappers
{
    public static TelegramBot.Models.LocationDto ToLocationDto(this TelegramBot.Data.Models.Location location)
    {
        return new LocationDto(location.City, location.State, location.Country.Name, location.Longitude, location.Latitude);
    }
}