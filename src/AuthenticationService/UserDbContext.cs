using Microsoft.EntityFrameworkCore;

namespace AuthenticationService
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public DbSet<UserDbModel> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserDbModel>().HasData(new UserDbModel()
            {
                UserId = 1,
                Username = "blake",
                Password = "Ylp2Zx3ukej77e0cwBHq3bUCWHfTQZK/ydIT2Qv35AM=",
                Country = "UK"
            });
        }
    }
}
