using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class BookingPetConfiguration : IEntityTypeConfiguration<BookingPet>
{
    public void Configure(EntityTypeBuilder<BookingPet> builder)
    {
        builder.HasIndex(x => new { x.ServiceBookingId, x.PetId }).IsUnique();

        builder.HasOne(x => x.ServiceBooking)
            .WithMany(x => x.BookingPets)
            .HasForeignKey(x => x.ServiceBookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Pet)
            .WithMany(x => x.BookingPets)
            .HasForeignKey(x => x.PetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

