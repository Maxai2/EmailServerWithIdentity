using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class EmailDbContext : IdentityDbContext
{
    public DbSet<User> Users {get; set;}

    public EmailDbContext(DbContextOptions<EmailDbContext> opts) : base(opts)
    {
        // Database.EnsureCreated();
    }
}