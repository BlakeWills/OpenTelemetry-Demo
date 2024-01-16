using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService;

public class AuthenticationService(UserDbContext userDbContext)
{
    public async Task<SignInResult> AuthenticateUser(BasicAuthenticationRequest request)
    {
        var hashedPassword = HashPassword(request.Username, request.Password);
        var user = await userDbContext.Users.SingleOrDefaultAsync(u => u.Username == request.Username && u.Password == hashedPassword);

        if(user != default)
        {
            return new SignInResult()
            {
                IsAuthenticated = true,
                User = new UserViewModel()
                {
                    Name = user.Username,
                    Country = user.Country
                }
            };
        }

        return new SignInResult()
        {
            IsAuthenticated = false
        };
    }

    private string HashPassword(string username, string password)
    {
        var salt = System.Text.Encoding.UTF8.GetBytes(username);

        // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 500_000,
            numBytesRequested: 256 / 8));
    }
}

public class SignInResult
{
    public required bool IsAuthenticated { get; set; }

    public UserViewModel? User { get; set; }
}
