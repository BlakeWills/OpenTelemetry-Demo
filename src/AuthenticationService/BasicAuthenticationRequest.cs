namespace AuthenticationService;

public record BasicAuthenticationRequest
{
    public required string Username { get; set; }

    public required string Password { get; set; }
}
