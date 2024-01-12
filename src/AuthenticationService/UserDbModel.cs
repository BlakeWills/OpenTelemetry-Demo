using System.ComponentModel.DataAnnotations;

namespace AuthenticationService;

public class UserDbModel
{
    [Key]
    public int UserId { get; set; }

    public required string Username { get; set; }

    public required string Password { get; set; }

    public required string Country { get; set; }
}
