using WeatherApp.Api.Core.DTOs;

namespace WeatherApp.Api.Core.Interfaces.Services
{
    public interface IWeatherService
    {
        Task<WeatherResponse?> GetCurrentWeatherAsync(string location);
        Task<WeatherForecastResponse?> GetWeatherForecastAsync(string location);
        Task<List<string>> GetPopularLocationsAsync();
        Task<bool> IsValidLocationAsync(string location);
    }
}