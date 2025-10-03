using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class ServiceOrderDetailConfiguration : IEntityTypeConfiguration<ServiceOrderDetail>
{
    public void Configure(EntityTypeBuilder<ServiceOrderDetail> builder)
    {

        builder.HasOne(x => x.ServiceOrder)
            .WithMany(x => x.OrderDetails)
            .HasForeignKey(x => x.ServiceOrderId)
            .OnDelete(DeleteBehavior.Cascade);


        // Fix the relationship with Slot - specify the navigation property on both sides
        builder.HasOne(x => x.Slot)
            .WithMany(x => x.OrderDetails)  // This links to the collection in Slot
            .HasForeignKey(x => x.SlotId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Service)
            .WithMany(x => x.OrderDetails)  // This links to the collection in Slot
            .HasForeignKey(x => x.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);



    }
}