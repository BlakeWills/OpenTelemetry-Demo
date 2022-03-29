using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace AuthenticationService
{
    public class AuthenticationService
    {
        private readonly string _connectionString;
        private readonly RandomNumberGenerator _random;

        public AuthenticationService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("UserDB");
            _random = RandomNumberGenerator.Create();
        }

        public async Task<SignInResult> AuthenticateUser(BasicAuthenticationRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();

            command.CommandText = "SELECT [UserName], [Country] FROM [USERS] WHERE UserName = @USERNAME AND PASSWORD = @PASSWORD";
            command.Parameters.AddWithValue("username", request.Username);
            command.Parameters.AddWithValue("password", HashPassword(request.Username, request.Password));

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (reader.Read())
            {
                return new SignInResult()
                {
                    IsAuthenticated = true,
                    User = new User()
                    {
                        Name = reader.GetString(0),
                        Country = reader.GetString(1)
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
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
        }
    }

    public class SignInResult
    {
        public bool IsAuthenticated { get; set; }

        public User User { get; set; }
    }
}
