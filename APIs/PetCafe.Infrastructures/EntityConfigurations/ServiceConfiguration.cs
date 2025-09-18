using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => new { x.ServiceType, x.IsActive });


        builder.Property(x => x.MaxParticipants)
            .HasDefaultValue(1);

        builder.Property(x => x.RequiresArea)
            .HasDefaultValue(true);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);
    }
}
