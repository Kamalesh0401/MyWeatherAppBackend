using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherApp.Api.Core.DTOs;
using WeatherApp.Api.Core.Interfaces.Services;

namespace WeatherApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet("current")]
        public async Task<ActionResult<WeatherResponse>> GetCurrentWeather([FromQuery] string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return BadRequest("Location is required");

            var weather = await _weatherService.GetCurrentWeatherAsync(location);
            if (weather == null)
                return NotFound("Weather data not found for the specified location");

            return Ok(weather);
        }

        [HttpGet("forecast")]
        public async Task<ActionResult<WeatherForecastResponse>> GetWeatherForecast([FromQuery] string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return BadRequest("Location is required");

            var forecast = await _weatherService.GetWeatherForecastAsync(location);
            if (forecast == null)
                return NotFound("Weather forecast not found for the specified location");

            return Ok(forecast);
        }

        [HttpGet("popular-locations")]
        public async Task<ActionResult<List<WeatherResponse>>> GetPopularLocations()
        {
            var locations = await _weatherService.GetPopularLocationsAsync();
            return Ok(locations);
        }

        [HttpGet("validate-location")]
        public async Task<ActionResult<bool>> ValidateLocation([FromQuery] string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return BadRequest("Location is required");

            var isValid = await _weatherService.IsValidLocationAsync(location);
            return Ok(new { isValid });
        }
    }
}