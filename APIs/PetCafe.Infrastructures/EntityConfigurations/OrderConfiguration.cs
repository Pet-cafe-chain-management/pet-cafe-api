using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasIndex(x => x.OrderNumber).IsUnique();
        builder.HasIndex(x => new { x.CustomerId, x.OrderDate });
        builder.HasIndex(x => x.OrderDate);

        builder.HasOne(x => x.Customer)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.PaymentStatus)
            .HasDefaultValue("Pending");

        builder.Property(x => x.Status)
            .HasDefaultValue("Processing");

        builder.Property(x => x.OrderDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
