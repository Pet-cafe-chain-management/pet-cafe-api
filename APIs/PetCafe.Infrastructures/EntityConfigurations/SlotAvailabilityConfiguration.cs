using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class SlotAvailabilityConfiguration : IEntityTypeConfiguration<SlotAvailability>
{
    public void Configure(EntityTypeBuilder<SlotAvailability> builder)
    {

        // Foreign key relationship with Slot
        builder.HasOne(sa => sa.Slot)
            .WithMany(x => x.SlotAvailabilities)
            .HasForeignKey(sa => sa.SlotId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

    }
}

