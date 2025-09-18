using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class ServiceBookingConfiguration : IEntityTypeConfiguration<ServiceBooking>
{
    public void Configure(EntityTypeBuilder<ServiceBooking> builder)
    {
        builder.HasIndex(x => x.BookingNumber).IsUnique();
        builder.HasIndex(x => new { x.CustomerId, x.BookingDate });
        builder.HasIndex(x => new { x.ScheduledDate, x.AreaId });
        builder.HasIndex(x => new { x.ServiceId, x.ScheduledDate });

        builder.HasOne(x => x.Customer)
            .WithMany(x => x.ServiceBookings)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Service)
            .WithMany(x => x.ServiceBookings)
            .HasForeignKey(x => x.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Area)
            .WithMany(x => x.ServiceBookings)
            .HasForeignKey(x => x.AreaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(x => x.Participants)
            .HasDefaultValue(1);


        builder.Property(x => x.PaymentStatus)
            .HasDefaultValue("Pending");

        builder.Property(x => x.BookingStatus)
            .HasDefaultValue("Pending");

        builder.Property(x => x.BookingDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}