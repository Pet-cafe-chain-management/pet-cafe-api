using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class ProductOrderDetailConfiguration : IEntityTypeConfiguration<ProductOrderDetail>
{
    public void Configure(EntityTypeBuilder<ProductOrderDetail> builder)
    {
        builder.HasIndex(x => new { x.ProductOrderId, x.ProductId });

        builder.HasOne(x => x.ProductOrder)
            .WithMany(x => x.OrderDetails)
            .HasForeignKey(x => x.ProductOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Product)
            .WithMany(x => x.OrderDetails)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.Property(x => x.IsForFeeding)
            .HasDefaultValue(false);
    }
}