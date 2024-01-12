using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;

namespace AuthenticationService.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController(AuthenticationService authenticationService) : ControllerBase
{
    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate()
    {
        var authHeader = Request.Headers.Authorization.ToString();

        if(!authHeader.StartsWith("Basic "))
        {
            return Unauthorized();
        }

        var header = AuthenticationHeaderValue.Parse(authHeader);
        var authInfo = Encoding.UTF8.GetString(Convert.FromBase64String(header.Parameter!)).Split(':');

        var request = new BasicAuthenticationRequest()
        {
            Username = authInfo[0],
            Password = authInfo[1]
        };

        var result = await authenticationService.AuthenticateUser(request);

        if(!result.IsAuthenticated)
        {
            return Unauthorized();
        }

        return Ok(result.User);
    }
}