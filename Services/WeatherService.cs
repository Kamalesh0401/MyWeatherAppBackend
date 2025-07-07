using Newtonsoft.Json;
using WeatherApp.Api.DTOs;
using WeatherApp.Api.Models;

namespace WeatherApp.Api.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public WeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _apiKey = _configuration["WeatherApiKey"] ?? "";
            _baseUrl = _configuration["WeatherApiBaseUrl"] ?? "";
        }

        public async Task<WeatherResponse?> GetCurrentWeatherAsync(string location)
        {
            try
            {
                var url = $"{_baseUrl}/weather?q={location}&appid={_apiKey}&units=metric";
                var response = await _httpClient.GetStringAsync(url);
                var weatherData = JsonConvert.DeserializeObject<dynamic>(response);

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
            catch (Exception)
            {
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
            catch (Exception)
            {
                return null;
            }
        }
    }
}