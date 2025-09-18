using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class PetSpecificConfiguration : IEntityTypeConfiguration<PetSpecific>
{
    public void Configure(EntityTypeBuilder<PetSpecific> builder)
    {
        builder.HasIndex(x => new { x.PetId, x.AttributeName });

        builder.HasOne(x => x.Pet)
            .WithMany(x => x.PetSpecifics)
            .HasForeignKey(x => x.PetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}