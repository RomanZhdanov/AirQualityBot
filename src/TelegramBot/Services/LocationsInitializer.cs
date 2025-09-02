using AirBro.TelegramBot.Data;
using AirBro.TelegramBot.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AirBro.TelegramBot.Services;

public class LocationsInitializer
{
   private readonly IAirQualityService _airService;
   private readonly ApplicationDbContext _dbContext;

   public LocationsInitializer(IAirQualityService airService, ApplicationDbContext dbContext)
   {
      _airService = airService;
      _dbContext = dbContext;
   }

   public async Task StartAsync()
   {
      var countries = await _airService.GetCountries();

      foreach (var country in countries)
      {
         var dbCountry = await _dbContext.Countries.SingleOrDefaultAsync(c => c.Name == country.Country);

         if (dbCountry is null)
         {
            _dbContext.Countries.Add(new Country()
            {
               Name = country.Country,
            });
         }
      }
      
      await _dbContext.SaveChangesAsync();
   }
}