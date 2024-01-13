using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        return await forecastClient.GetForecastForUser(user);
    }
}
