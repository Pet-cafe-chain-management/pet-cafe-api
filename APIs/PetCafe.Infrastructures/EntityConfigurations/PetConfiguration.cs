using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Constants;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {

        builder.Property(x => x.HealthStatus)
            .HasDefaultValue(HealthStatusConstant.HEALTHY);

        builder.HasOne(x => x.Breed)
            .WithMany(x => x.Pets)
            .HasForeignKey(x => x.BreedId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}





