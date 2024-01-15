using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
    private static readonly Dictionary<int, string> _summaries = new()
    {
        { -30, "the start of Ice Age 5. Say hi to Sid" },
        { -20, "really, really cold" },
        { -10, "really cold" },
        { 0, "cold" },
        { 10, "meh" },
        { 20, "great for a BBQ" },
        { 30, "a little too warm" },
        { 50, "oh god my face is melting" },
    };

    public WeatherForecast Get(User user)
    {
        logger.LogInformation("Generating forecast for {user} in {country}", user.Name, user.Country);

        var temperature = GetTemperature();
        var summary = GetSummary(temperature);

        return new WeatherForecast
        {
            Date = DateTime.UtcNow,
            Description = $"Hi {user.Name}! Today in {user.Country} it's going to be {summary}.",
            TemperatureC = temperature,
            Summary = summary
        };
    }

    private static int GetTemperature()
    {
        // The core of our really high tech, proprietary weather forecasting algorithm.
        var temperature = Random.Shared.Next(-20, 55);

        Activity.Current?.AddTag("forecast.temperatureC", temperature);

        return temperature;
    }

    private static string GetSummary(int temp)
    {
        return _summaries[((temp / 10)) * 10];
    }
}