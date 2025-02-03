using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MikesWallet.Accounts.WebApi.DAL.Models;

public class Withdrawal : Operation
{
    public required Guid AccountId { get; init; }
    
    public Account Account { get; init; } = null!;
}

public class WithdrawalConfiguration : IEntityTypeConfiguration<Withdrawal>
{
    public void Configure(EntityTypeBuilder<Withdrawal> builder)
    {
        builder.UseTptMappingStrategy();

        builder.HasOne(e => e.Account)
            .WithMany()
            .HasForeignKey(e => e.AccountId);
    }
}