using Microsoft.AspNetCore.Mvc;

namespace ForecastService.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(ForecastProvider forecastProvider) : ControllerBase
{
    [HttpGet("GetForecastForUser")]
    public WeatherForecast Get(User user)
    {
        return forecastProvider.Get(user);
    }
}

public class ForecastProvider(ILogger<WeatherForecastController> logger)
{
    private static readonly string[] _summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    public WeatherForecast Get(User user)
    {
        logger.LogInformation("Generating forecast for {user} in {country}", user.Name, user.Country);

        // The core of our really high tech, proprietary weather forecasting algorithm.
        var temperature = Random.Shared.Next(-20, 55);
        var summary = _summaries[Random.Shared.Next(_summaries.Length)];

        return new WeatherForecast
        {
            Date = DateTime.UtcNow,
            Description = $"Hi {user.Name}! Today in {user.Country} it's going to be {summary}.",
            TemperatureC = temperature,
            Summary = summary
        };
    }
}