using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasIndex(x => new { x.AccountId, x.IsRead });
        builder.HasIndex(x => new { x.NotificationType, x.CreatedAt });
        builder.HasIndex(x => x.ScheduledSendDate);

        builder.HasOne(x => x.Account)
            .WithMany(x => x.Notifications)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Priority)
            .HasDefaultValue("Normal");

        builder.Property(x => x.IsRead)
            .HasDefaultValue(false);
    }
}
