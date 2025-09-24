using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class VaccineTypeConfiguration : IEntityTypeConfiguration<VaccineType>
{
    public void Configure(EntityTypeBuilder<VaccineType> builder)
    {
        builder.HasIndex(x => new { x.Name }).IsUnique();

        builder.Property(x => x.IsRequired)
            .HasDefaultValue(false);

        builder.HasOne(x => x.Species)
            .WithMany(x => x.VaccineTypes)
            .HasForeignKey(x => x.SpeciesId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
