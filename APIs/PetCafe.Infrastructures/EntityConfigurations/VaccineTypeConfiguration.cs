using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class VaccineTypeConfiguration : IEntityTypeConfiguration<VaccineType>
{
    public void Configure(EntityTypeBuilder<VaccineType> builder)
    {
        builder.HasIndex(x => new { x.Name, x.Species }).IsUnique();

        builder.Property(x => x.IsMandatory)
            .HasDefaultValue(false);
    }
}
