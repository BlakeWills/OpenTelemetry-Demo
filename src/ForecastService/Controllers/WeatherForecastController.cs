using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ForecastService.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(ForecastProvider forecastProvider) : ControllerBase
{
    [HttpGet("GetForecastForUser")]
    public async Task<WeatherForecast> Get(User user)
    {
        return await forecastProvider.Get(user);
    }
}

public class ForecastProvider(ILogger<WeatherForecastController> logger)
{
    private static readonly Dictionary<int, string> _summaries = new()
    {
        { -30, "very, very, very cold" },
        { -20, "very, very cold" },
        { -10, "very cold" },
        { 0, "cold" },
        { 10, "meh" },
        { 20, "warm" },
        { 30, "really warm" },
        { 50, "really, really warm" }
    };

    public async Task<WeatherForecast> Get(User user)
    {
        logger.LogInformation("Generating forecast for {user} in {country}", user.Name, user.Country);

        var temperature = await GetForecast(user.Country);
        var summary = GetSummary(temperature);

        return new WeatherForecast
        {
            Date = DateTime.UtcNow,
            Greeting = $"Hi {user.Name}! Today in {user.Country} it's going to be {summary}.",
            TemperatureC = temperature,
            Summary = summary
        };
    }

    /// <summary>
    /// The core of our really high tech, proprietary weather forecasting algorithm.
    /// </summary>
    /// <param name="country"></param>
    private static async Task<int> GetForecast(string country)
    {
        int temperature;

        if (country.Equals("uk", StringComparison.CurrentCultureIgnoreCase))
        {
            // Nice and fast because we can assume it's always raining.
            temperature = Random.Shared.Next(-10, 40);
        }
        else
        {
            // Actually do some work
            await Task.Delay(millisecondsDelay: Random.Shared.Next(0, 500));

            temperature = Random.Shared.Next(-20, 55);
        }

        Activity.Current?.AddTag("forecast.temperatureC", temperature);

        return temperature;
    }

    private static string GetSummary(int temp)
    {
        return _summaries[temp / 10 * 10];
    }
}