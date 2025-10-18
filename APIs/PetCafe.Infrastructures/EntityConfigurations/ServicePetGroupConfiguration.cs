using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class ServicePetGroupConfiguration : IEntityTypeConfiguration<ServicePetGroup>
{
    public void Configure(EntityTypeBuilder<ServicePetGroup> builder)
    {

        builder.HasOne(x => x.PetGroup)
            .WithMany(x => x.ServicePetGroups)
            .HasForeignKey(x => x.PetGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Service)
            .WithMany(x => x.ServicePetGroups)
            .HasForeignKey(x => x.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}