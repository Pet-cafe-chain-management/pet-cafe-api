using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class AreaServiceConfiguration : IEntityTypeConfiguration<AreaService>
{
    public void Configure(EntityTypeBuilder<AreaService> builder)
    {
        builder.HasIndex(x => new { x.AreaId, x.ServiceId }).IsUnique();

        builder.HasOne(x => x.Area)
            .WithMany(x => x.AreaServices)
            .HasForeignKey(x => x.AreaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Service)
            .WithMany(x => x.AreaServices)
            .HasForeignKey(x => x.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.Property(x => x.IsAvailable)
            .HasDefaultValue(true);
    }
}