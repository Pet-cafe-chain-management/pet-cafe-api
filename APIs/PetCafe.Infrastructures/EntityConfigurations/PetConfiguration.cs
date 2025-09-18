using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.HasIndex(x => x.Name);

        builder.HasOne(x => x.PetBreed)
            .WithMany(x => x.Pets)
            .HasForeignKey(x => x.PetBreedId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PetStatus)
            .WithMany(x => x.Pets)
            .HasForeignKey(x => x.PetStatusId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.Property(x => x.ArrivalDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}





