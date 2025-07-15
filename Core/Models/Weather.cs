namespace WeatherApp.Api.Core.Models
{
    public class WeatherData
    {
        public string LocationName { get; set; } = string.Empty;
        public double Temperature { get; set; }
        public double FeelsLike { get; set; }
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
    }

    public class ForecastData
    {
        public string LocationName { get; set; } = string.Empty;
        public List<DailyForecast> Forecast { get; set; } = new();
    }

    public class DailyForecast
    {
        public DateTime Date { get; set; }
        public double TempMax { get; set; }
        public double TempMin { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }
    }
}