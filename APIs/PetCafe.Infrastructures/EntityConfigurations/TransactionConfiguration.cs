using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {


        builder.HasOne(x => x.Order)
            .WithMany(x => x.Transactions)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.SetNull);

    }
}

