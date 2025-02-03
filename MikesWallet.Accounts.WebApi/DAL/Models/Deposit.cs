using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MikesWallet.Accounts.WebApi.DAL.Models;

public class Deposit : Operation
{
    public required Guid AccountId { get; init; }
    
    public Account Account { get; init; } = null!;
}

public class DepositConfiguration : IEntityTypeConfiguration<Deposit>
{
    public void Configure(EntityTypeBuilder<Deposit> builder)
    {
        builder.UseTptMappingStrategy();

        builder.HasOne(d => d.Account)
            .WithMany()
            .HasForeignKey(d => d.AccountId);
    }
}