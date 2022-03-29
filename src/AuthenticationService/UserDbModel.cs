using System.ComponentModel.DataAnnotations;

namespace AuthenticationService
{
    public class UserDbModel
    {
        [Key]
        public int UserId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Country { get; set; }
    }
}
