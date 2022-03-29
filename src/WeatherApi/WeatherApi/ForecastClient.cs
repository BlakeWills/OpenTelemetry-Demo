using System.Text.Json;

namespace WeatherApi
{
    public class ForecastClient
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _forecastServiceUrl;

        public ForecastClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _forecastServiceUrl = new Uri(configuration.GetConnectionString("ForecastService"));
        }

        public async Task<WeatherForecast> GetForecastForUser(User user)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri(_forecastServiceUrl, "getforecastforuser"))
            {
                Content = JsonContent.Create(user)
            };

            var response = await _httpClient.SendAsync(message);
            response.EnsureSuccessStatusCode();

            var forecastJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<WeatherForecast>(forecastJson);
        }
    }
}
