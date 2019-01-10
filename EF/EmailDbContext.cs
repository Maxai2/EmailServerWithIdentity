using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class EmailDbContext : IdentityDbContext<User>
{
    public DbSet<Mail> Mails { get; set; }
    public DbSet<AccountToken> Tokens { get; set; }

    public EmailDbContext(DbContextOptions<EmailDbContext> opts) : base(opts)
    {}
}