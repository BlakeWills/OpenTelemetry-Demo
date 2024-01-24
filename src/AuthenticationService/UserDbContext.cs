using Microsoft.EntityFrameworkCore;

namespace AuthenticationService;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public DbSet<UserDbModel> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserDbModel>().HasData(new UserDbModel()
        {
            UserId = 1,
            Username = "bob",
            Password = "Q+TS3JsOPi6pvtIn/FazPm+B8HdbzhutGwJulxHgPVc=",
            Country = "UK"
        },
        new UserDbModel()
        {
            UserId = 2,
            Username = "alice",
            Password = "hWvFo5OctEp0K/j/RNR1RTGQsG7yATpWVI0T1GZYt0k=", // p@55w0rd (500k iterations)
            Country = "Singapore"
        });
    }
}
