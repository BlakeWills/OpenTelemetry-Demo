using Microsoft.AspNetCore.Mvc;

namespace WeatherApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly AuthenticationClient _authClient;
        private readonly ForecastClient _forecastClient;

        public WeatherForecastController(
            AuthenticationClient authClient,
            ForecastClient forecastClient)
        {
            _authClient = authClient;
            _forecastClient = forecastClient;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<WeatherForecast> Get()
        {
            var user = await _authClient.AuthenticateUser(Request);
            return await _forecastClient.GetForecastForUser(user);
        }
    }
}