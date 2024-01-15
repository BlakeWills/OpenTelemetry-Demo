using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace WeatherApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(
    ForecastClient forecastClient) : ControllerBase
{
    [Authorize]
    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<WeatherForecast> Get()
    {
        var user = HttpContext.GetCurrentUser();

        Activity.Current?.AddTag("user.country", user.Country);
        Activity.Current?.AddBaggage("user.country", user.Country);

        return await forecastClient.GetForecastForUser(user);
    }
}
