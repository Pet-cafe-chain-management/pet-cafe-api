using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class ServiceOrderConfiguration : IEntityTypeConfiguration<ServiceOrder>
{
    public void Configure(EntityTypeBuilder<ServiceOrder> builder)
    {
        builder.HasIndex(x => x.OrderDate);

        builder.HasOne(x => x.Order)
            .WithOne(x => x.ServiceOrder)
            .HasForeignKey<ServiceOrder>(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);


    }
}
