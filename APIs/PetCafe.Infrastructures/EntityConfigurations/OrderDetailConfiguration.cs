using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
{
    public void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
        builder.HasIndex(x => new { x.OrderId, x.ProductId });

        builder.HasOne(x => x.Order)
            .WithMany(x => x.OrderDetails)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Product)
            .WithMany(x => x.OrderDetails)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Fix the relationship with Slot - specify the navigation property on both sides
        builder.HasOne(x => x.Slot)
            .WithMany(x => x.OrderDetails)  // This links to the collection in Slot
            .HasForeignKey(x => x.SlotId)
            .OnDelete(DeleteBehavior.Cascade);



        builder.Property(x => x.IsForFeeding)
            .HasDefaultValue(false);
    }
}