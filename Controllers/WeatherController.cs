using Microsoft.AspNetCore.Mvc;
using WeatherApp.Api.Services;

namespace WeatherApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weatherService;

        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet("current/{location}")]
        public async Task<IActionResult> GetCurrentWeather(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return BadRequest("Location is required");
            }

            var weather = await _weatherService.GetCurrentWeatherAsync(location);
            if (weather == null)
            {
                return NotFound($"Weather data not found for location: {location}");
            }

            return Ok(weather);
        }

        [HttpGet("forecast/{location}")]
        public async Task<IActionResult> GetWeatherForecast(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return BadRequest("Location is required");
            }

            var forecast = await _weatherService.GetWeatherForecastAsync(location);
            if (forecast == null)
            {
                return NotFound($"Weather forecast not found for location: {location}");
            }

            return Ok(forecast);
        }
    }
}