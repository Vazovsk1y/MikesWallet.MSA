using Microsoft.EntityFrameworkCore;
using MikesWallet.Users.WebApi.DAL.Models;

namespace MikesWallet.Users.WebApi.DAL;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    private const string CaseInsensitiveCollationName = "case_insensitive_collation";
    
    public DbSet<User> Users { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasCollation(CaseInsensitiveCollationName, locale: "und-u-ks-level1", provider: "icu", deterministic: false);

        modelBuilder.Entity<User>(u =>
        {
            u.Property(e => e.Email)
                .UseCollation(CaseInsensitiveCollationName);
            
            u.HasIndex(e => e.Email)
                .IsUnique();
        });
    }
}