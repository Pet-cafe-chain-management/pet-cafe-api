using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class PetBreedConfiguration : IEntityTypeConfiguration<PetBreed>
{
    public void Configure(EntityTypeBuilder<PetBreed> builder)
    {
        builder.HasIndex(x => new { x.Name, x.SpeciesId }).IsUnique();

        builder.HasOne(x => x.Species)
            .WithMany(x => x.PetBreeds)
            .HasForeignKey(x => x.SpeciesId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}