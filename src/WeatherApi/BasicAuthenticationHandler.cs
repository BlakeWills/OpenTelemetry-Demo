using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace WeatherApi;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Uri _authServiceUrl;

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web);

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration) : base(options, logger, encoder)
    {
        _httpClientFactory = httpClientFactory;
        _authServiceUrl = new(configuration.GetConnectionString("AuthenticationService"));
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (string.IsNullOrWhiteSpace(Request.Headers.Authorization))
        {
            return AuthenticateResult.Fail("Invalid Authorization Header");
        }

        if(!AuthenticationHeaderValue.TryParse(Request.Headers.Authorization, out var parsedValue))
        {
            return AuthenticateResult.Fail("Invalid Authorization Header");
        }

        using var httpClient = _httpClientFactory.CreateClient("UnsafeHttpClient");

        var message = new HttpRequestMessage(HttpMethod.Post, new Uri(_authServiceUrl, "authentication/authenticate"));
        message.Headers.Authorization = parsedValue;

        var response = await httpClient.SendAsync(message);
        response.EnsureSuccessStatusCode();

        var user = await JsonSerializer.DeserializeAsync<User>(
            await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions)
            ?? throw new InvalidOperationException("Failed to fetch user from auth service");

        var claims = new[]
        { 
            new Claim("name", user.Name),
            new Claim("country", user.Country)
        };

        var identity = new ClaimsIdentity(claims, "Basic");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
    }
}
