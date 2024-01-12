namespace WeatherApi.Controllers;

internal static class HttpContextExtensions
{
    public static User GetCurrentUser(this HttpContext context)
    {
        return new User
        {
            Name = context.User.Claims.FirstOrDefault(x => x.Type == "name").Value,
            Country = context.User.Claims.FirstOrDefault(x => x.Type == "country").Value
        };
    }
}