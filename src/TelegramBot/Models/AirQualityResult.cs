namespace AirBro.TelegramBot.Models;

public class AirQualityResult
{
    public Location Location { get; set; }

    public int Aqi { get; set; }

    public DateTime LastUpdate { get; set; }

    public string Quality
    {
        get
        {
            return Aqi switch
            {
                <= 50 => "Good",
                > 50 and <= 100 => "Moderate",
                > 100 and <= 150 => "Unhealthy for sensitive groups",
                > 150 and <= 200 => "Unhealthy",
                > 200 and <= 300 => "Vary Unhealthy",
                > 300 and <= 500 => "Hazardous",
                _ => "Unknown"
            };
        }
    }
}