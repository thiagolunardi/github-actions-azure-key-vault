using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace DevSecOps.Data
{
    public class WeatherForecastService
    {
        private readonly IDistributedCache _cache;

        public WeatherForecastService(IDistributedCache cache)
            => _cache = cache;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public async Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
        {
            var weather = await _cache.GetStringAsync("weather");

            if (weather == null)
            {
                var rng = new Random();
                var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();

                weather = JsonSerializer.Serialize(forecasts);

                await _cache.SetStringAsync("weather", weather, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5)
                });
            }
            var weatherForecast = (WeatherForecast[])JsonSerializer
                .Deserialize(weather, typeof(WeatherForecast[]));

            return weatherForecast;
        }
    }
}
