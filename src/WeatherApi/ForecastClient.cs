using System.Net.Http.Headers;
using System.Text.Json;

namespace WeatherApi
{
    public class ForecastClient
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _forecastServiceUrl;

        public ForecastClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("UnsafeHttpClient");
            _forecastServiceUrl = new Uri(configuration.GetConnectionString("ForecastService"));
        }

        public async Task<WeatherForecast> GetForecastForUser(User user)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri(_forecastServiceUrl, "weatherforecast/GetForecastForUser"))
            {
                Content = JsonContent.Create(user, mediaType: MediaTypeHeaderValue.Parse("application/json"))
            };

            var response = await _httpClient.SendAsync(message);
            response.EnsureSuccessStatusCode();

            var forecastJson = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            return JsonSerializer.Deserialize<WeatherForecast>(forecastJson, options);
        }
    }
}
