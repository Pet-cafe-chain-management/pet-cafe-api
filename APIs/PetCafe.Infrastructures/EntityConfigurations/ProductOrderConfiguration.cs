using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class ProductOrderConfiguration : IEntityTypeConfiguration<ProductOrder>
{
    public void Configure(EntityTypeBuilder<ProductOrder> builder)
    {

        builder.HasOne(x => x.Order)
            .WithOne(x => x.ProductOrder)
            .HasForeignKey<ProductOrder>(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);


    }
}
