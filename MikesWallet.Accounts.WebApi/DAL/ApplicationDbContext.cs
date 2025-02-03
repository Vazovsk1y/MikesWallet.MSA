using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MikesWallet.Accounts.WebApi.DAL.Models;

namespace MikesWallet.Accounts.WebApi.DAL;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; init; }
    
    public DbSet<Operation> Operations { get; init; }
    
    public DbSet<Transfer> Transfers { get; init; }
    
    public DbSet<Withdrawal> Withdrawals { get; init; }
    
    public DbSet<Deposit> Deposits { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}