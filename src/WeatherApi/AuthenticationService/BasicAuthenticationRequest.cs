namespace AuthenticationService
{
    public record BasicAuthenticationRequest
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
}
