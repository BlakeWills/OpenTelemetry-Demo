using System.Net.Http.Headers;
using System.Text.Json;

namespace WeatherApi;

public class ForecastClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("UnsafeHttpClient");
    private readonly Uri _forecastServiceUrl = new(configuration.GetConnectionString("ForecastService"));

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web);

    public async Task<WeatherForecast> GetForecastForUser(User user)
    {
        var message = new HttpRequestMessage(HttpMethod.Get, new Uri(_forecastServiceUrl, "weatherforecast/GetForecastForUser"))
        {
            Content = JsonContent.Create(user, mediaType: MediaTypeHeaderValue.Parse("application/json"))
        };

        var response = await _httpClient.SendAsync(message);
        response.EnsureSuccessStatusCode();

        var forecastJson = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<WeatherForecast>(forecastJson, _jsonSerializerOptions)
            ?? throw new InvalidOperationException($"Unable to deserialize forecast: {forecastJson}");
    }
}
