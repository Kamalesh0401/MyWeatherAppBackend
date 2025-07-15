using Newtonsoft.Json;
using WeatherApp.Api.Core.DTOs;
using WeatherApp.Api.Core.Interfaces.Services;

namespace WeatherApp.Api.Infrastructure.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(HttpClient httpClient, IConfiguration configuration, ILogger<WeatherService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _apiKey = _configuration["WeatherApiKey"] ?? throw new InvalidOperationException("WeatherApiKey not configured");
            _baseUrl = _configuration["WeatherApiBaseUrl"] ?? throw new InvalidOperationException("WeatherApiBaseUrl not configured");
        }

        public async Task<WeatherResponse?> GetCurrentWeatherAsync(string location)
        {
            try
            {
                var url = $"{_baseUrl}/weather?q={location}&appid={_apiKey}&units=metric";
                var response = await _httpClient.GetStringAsync(url);
                var weatherData = JsonConvert.DeserializeObject<dynamic>(response);

                if (weatherData == null) return null;

                return new WeatherResponse
                {
                    LocationName = weatherData.name,
                    Temperature = weatherData.main.temp,
                    FeelsLike = weatherData.main.feels_like,
                    Humidity = weatherData.main.humidity,
                    WindSpeed = weatherData.wind.speed,
                    Description = weatherData.weather[0].description,
                    Icon = weatherData.weather[0].icon,
                    DateTime = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current weather for location: {Location}", location);
                return null;
            }
        }

        public async Task<WeatherForecastResponse?> GetWeatherForecastAsync(string location)
        {
            try
            {
                var url = $"{_baseUrl}/forecast?q={location}&appid={_apiKey}&units=metric";
                var response = await _httpClient.GetStringAsync(url);
                var forecastData = JsonConvert.DeserializeObject<dynamic>(response);

                if (forecastData == null) return null;

                var forecast = new WeatherForecastResponse
                {
                    LocationName = forecastData.city.name,
                    Forecast = new List<ForecastDay>()
                };

                var dailyForecasts = new Dictionary<string, ForecastDay>();

                foreach (var item in forecastData.list)
                {
                    var date = DateTime.Parse(item.dt_txt.ToString()).Date;
                    var dateKey = date.ToString("yyyy-MM-dd");

                    if (!dailyForecasts.ContainsKey(dateKey))
                    {
                        dailyForecasts[dateKey] = new ForecastDay
                        {
                            Date = date,
                            TempMax = item.main.temp_max,
                            TempMin = item.main.temp_min,
                            Description = item.weather[0].description,
                            Icon = item.weather[0].icon,
                            Humidity = item.main.humidity,
                            WindSpeed = item.wind.speed
                        };
                    }
                    else
                    {
                        var existing = dailyForecasts[dateKey];
                        existing.TempMax = Math.Max(existing.TempMax, (double)item.main.temp_max);
                        existing.TempMin = Math.Min(existing.TempMin, (double)item.main.temp_min);
                    }
                }

                forecast.Forecast = dailyForecasts.Values.OrderBy(f => f.Date).Take(5).ToList();
                return forecast;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting weather forecast for location: {Location}", location);
                return null;
            }
        }

        public async Task<List<string>> GetPopularLocationsAsync()
        {
            // This could be enhanced to store popular locations in database
            return new List<string>
            {
                "New York",
                "London",
                "Tokyo",
                "Paris",
                "Sydney",
                "Mumbai",
                "Berlin",
                "Toronto",
                "Dubai",
                "Singapore"
            };
        }

        public async Task<bool> IsValidLocationAsync(string location)
        {
            try
            {
                var weather = await GetCurrentWeatherAsync(location);
                return weather != null;
            }
            catch
            {
                return false;
            }
        }
    }
}