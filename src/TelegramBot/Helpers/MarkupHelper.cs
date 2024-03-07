using AirBro.TelegramBot.Models;
using IQAirApiClient.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace AirBro.TelegramBot.Helpers;

public static class MarkupHelper
{
    public static InlineKeyboardMarkup GetCountriesPage(PaginatedList<CountryItem> countriesPage)
    {
        var buttonRows = new List<List<InlineKeyboardButton>>();
        foreach (var country in countriesPage.Items)
        {
            var buttonRow = new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData(country.Country, $"/add_location?set_country|{country.Country}")
            };
            buttonRows.Add(buttonRow);
        }

        var navigationButtons = new List<InlineKeyboardButton>();

        if (countriesPage.HasPreviousPage)
        {
            navigationButtons.Add(InlineKeyboardButton.WithCallbackData("<<", $"/add_location?countries_page|{countriesPage.PageNumber - 1}"));
        }

        if (countriesPage.HasNextPage)
        {
            navigationButtons.Add(InlineKeyboardButton.WithCallbackData(">>", $"/add_location?countries_page|{countriesPage.PageNumber + 1}"));
        }
        
        buttonRows.Add(navigationButtons);
        
        return new InlineKeyboardMarkup(buttonRows);
    }
    
    public static InlineKeyboardMarkup GetStatesPage(string country, PaginatedList<StateItem> statesPage)
    {
        var buttonRows = new List<List<InlineKeyboardButton>>();
        foreach (var state in statesPage.Items)
        {
            var buttonRow = new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData(state.State, $"/add_location?set_state|{state.State}")
            };
            buttonRows.Add(buttonRow);
        }

        var navigationButtons = new List<InlineKeyboardButton>();

        if (statesPage.HasPreviousPage)
        {
            navigationButtons.Add(InlineKeyboardButton.WithCallbackData("<<", $"/add_location?|states_page|{country}|{statesPage.PageNumber - 1}"));
        }

        if (statesPage.HasNextPage)
        {
            navigationButtons.Add(InlineKeyboardButton.WithCallbackData(">>", $"/add_location?states_page|{country}|{statesPage.PageNumber + 1}"));
        }
        
        buttonRows.Add(navigationButtons);
        
        return new InlineKeyboardMarkup(buttonRows);
    }

    public static InlineKeyboardMarkup GetCitiesPage(string country, string state, PaginatedList<CityItem> citiesPage)
    {
        var buttonRows = new List<List<InlineKeyboardButton>>();
        foreach (var city in citiesPage.Items)
        {
            var buttonRow = new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData(city.City, $"/add_location?set_city|{city.City}")
            };
            buttonRows.Add(buttonRow);
        }

        var navigationButtons = new List<InlineKeyboardButton>();

        if (citiesPage.HasPreviousPage)
        {
            navigationButtons.Add(InlineKeyboardButton.WithCallbackData("<<", $"/add_location?cities_page|{country}|{state}|{citiesPage.PageNumber - 1}"));
        }

        if (citiesPage.HasNextPage)
        {
            navigationButtons.Add(InlineKeyboardButton.WithCallbackData(">>", $"/add_location?cities_page|{country}|{state}|{citiesPage.PageNumber + 1}"));
        }
        
        buttonRows.Add(navigationButtons);
        
        return new InlineKeyboardMarkup(buttonRows);
    }
}