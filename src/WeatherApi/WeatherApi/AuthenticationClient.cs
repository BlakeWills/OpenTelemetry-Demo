using System.Net.Http.Headers;
using System.Text.Json;

namespace WeatherApi
{
    public class AuthenticationClient
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _authServiceUrl;

        public AuthenticationClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _authServiceUrl = new Uri(configuration.GetConnectionString("AuthenticationService"));
        }

        public async Task<User> AuthenticateUser(HttpRequest request)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri(_authServiceUrl, "authentication/authenticate"));
            message.Headers.Authorization = AuthenticationHeaderValue.Parse(request.Headers.Authorization);

            var response = await _httpClient.SendAsync(message);
            response.EnsureSuccessStatusCode();

            var userJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<User>(userJson);
        }
    }
}
