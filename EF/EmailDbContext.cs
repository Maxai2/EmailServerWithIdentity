using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class EmailDbContext : IdentityDbContext
{
    public DbSet<User> Users {get; set;}
    public DbSet<Mail> Mails { get; set; }
    public DbSet<AccountToken> Tokens { get; set; }

    public EmailDbContext(DbContextOptions<EmailDbContext> opts) : base(opts)
    {
        // Database.EnsureCreated();
    }
}