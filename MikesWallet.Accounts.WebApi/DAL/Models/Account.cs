using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MikesWallet.Accounts.WebApi.DAL.Models;

public class Account
{
    public required Guid Id { get; init; }
    
    public required Guid UserId { get; init; }
    
    public required string Name { get; init; }
    
    public required DateTimeOffset CreationDateTime { get; init; }
    
    public required Currency Currency { get; init; }
    
    /*
     Normally account balance should be calculated with addition/subtraction operations related to this account.
     To improve performance(read) this column will store actual balance which I will change all the time the new operation created. 
    */
    public required decimal Balance { get; set; }
}

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.Property(e => e.Name);
        
        builder
            .Property(e => e.Currency)
            .HasConversion(e => e.ToString(), e => Enum.Parse<Currency>(e));

        builder
            .HasIndex(e => new { e.UserId, e.Currency })
            .IsUnique();
        
        builder
            .Property(e => e.Balance)
            .HasPrecision(19, 2);

        builder.ToTable(t => t.HasCheckConstraint(
            $"CK_{nameof(Account)}_{nameof(Account.Balance)}", 
            $"\"{nameof(Account.Balance)}\" >= 0"));
    }
}

// ReSharper disable InconsistentNaming
public enum Currency
{
    [Display(Name = "Pound Sterling")]
    GBP,

    [Display(Name = "Kazakhstani Tenge")]
    KZT,

    [Display(Name = "Polish Złoty")]
    PLN,
    
    [Display(Name = "United States Dollar")]
    USD,

    [Display(Name = "Ukrainian Hryvnia")]
    UAH,

    [Display(Name = "Russian Ruble")]
    RUB,
}
