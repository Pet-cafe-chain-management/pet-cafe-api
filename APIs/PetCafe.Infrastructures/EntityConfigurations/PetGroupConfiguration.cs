using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class PetGroupConfiguration : IEntityTypeConfiguration<PetGroup>
{
    public void Configure(EntityTypeBuilder<PetGroup> builder)
    {

        builder.HasOne(x => x.PetBreed)
            .WithMany()
            .HasForeignKey(x => x.PetBreedId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PetSpecies)
            .WithMany()
            .HasForeignKey(x => x.PetSpeciesId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}