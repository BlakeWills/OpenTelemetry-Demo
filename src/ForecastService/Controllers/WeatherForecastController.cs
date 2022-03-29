using Microsoft.AspNetCore.Mvc;

namespace ForecastService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ForecastProvider _forecastProvider;

        public WeatherForecastController(ForecastProvider forecastProvider)
        {
            _forecastProvider = forecastProvider;
        }

        [HttpGet("GetForecastForUser")]
        public WeatherForecast Get(User user)
        {
            return _forecastProvider.Get(user);
        }
    }

    public class ForecastProvider
    {
        private static readonly string[] _summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public ForecastProvider(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        public WeatherForecast Get(User user)
        {
            _logger.LogInformation("Generating forecast for {user} in {country}", user.Name, user.Country);

            var summary = _summaries[Random.Shared.Next(_summaries.Length)];

            return new WeatherForecast
            {
                Date = DateTime.Now,
                Description = $"Hi {user.Name}! Today in {user.Country} it's going to be {summary}.",
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = summary
            };
        }
    }
}