using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MikesWallet.Accounts.WebApi.DAL.Models;

public abstract class Operation
{
    public required Guid Id { get; init; }
    
    public required DateTimeOffset CreationDateTime { get; init;}
    
    public required bool Income { get; init; }
    
    public required decimal Amount { get; init; }
    
    public required decimal Commission { get; init; }
    
    public required OperationType Type { get; init; }
}

public class OperationConfiguration : IEntityTypeConfiguration<Operation>
{
    public void Configure(EntityTypeBuilder<Operation> builder)
    {
        builder.UseTptMappingStrategy();
        
        builder.Property(e => e.Type)
            .HasConversion(e => e.ToString(), e => Enum.Parse<OperationType>(e));

        builder.Property(e => e.Amount)
            .HasPrecision(19, 2);
        
        builder.Property(e => e.Commission)
            .HasPrecision(19, 2);
    }
}

public enum OperationType
{
    Withdrawal,
    Deposit,
    Transfer
}
