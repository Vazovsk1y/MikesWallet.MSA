using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MikesWallet.Accounts.WebApi.DAL.Models;

public class Transfer : Operation
{
    public required Guid AccountFromId { get; init; }
    
    public required Guid AccountToId { get; init; }
    
    public required decimal ExchangeRate { get; init; }
    
    public Account To { get; init; } = null!;

    public Account From { get; init; } = null!;
}

public class TransferConfiguration : IEntityTypeConfiguration<Transfer>
{
    public void Configure(EntityTypeBuilder<Transfer> builder)
    {
        builder.UseTptMappingStrategy();

        builder.Property(e => e.ExchangeRate)
            .HasPrecision(19, 4);

        builder.HasOne(e => e.From)
            .WithMany()
            .HasForeignKey(e => e.AccountFromId);
        
        builder.HasOne(e => e.To)
            .WithMany()
            .HasForeignKey(e => e.AccountToId);
    }
}