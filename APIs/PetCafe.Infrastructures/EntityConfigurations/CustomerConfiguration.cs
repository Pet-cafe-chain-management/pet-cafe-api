using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {

        builder.HasOne(x => x.Account)
            .WithOne(x => x.Customer)
            .HasForeignKey<Customer>(x => x.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}