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
            Username = "blake",
            Password = "BU7XwhE8WoCzAQZM0KprvaeixSdhrmfS301z+G6mSq4=", // p@55w0rd
            Country = "UK"
        });
    }
}
