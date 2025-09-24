using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.HasIndex(x => x.Name);

        builder.HasOne(x => x.Breed)
            .WithMany(x => x.Pets)
            .HasForeignKey(x => x.BreedId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}





