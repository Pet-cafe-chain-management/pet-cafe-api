using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class CustomerBookingConfiguration : IEntityTypeConfiguration<CustomerBooking>
{
    public void Configure(EntityTypeBuilder<CustomerBooking> builder)
    {
        builder.HasIndex(x => new { x.CustomerId, x.BookingDate });
        builder.HasIndex(x => new { x.ScheduledDate });
        builder.HasIndex(x => new { x.ServiceId, x.ScheduledDate });

        builder.HasOne(x => x.Customer)
            .WithMany(x => x.Bookings)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Service)
            .WithMany(x => x.Bookings)
            .HasForeignKey(x => x.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.OrderDetail)
            .WithOne(x => x.Booking)
            .HasForeignKey<CustomerBooking>(x => x.OrderDetailId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.Property(x => x.Participants)
            .HasDefaultValue(1);

    }
}