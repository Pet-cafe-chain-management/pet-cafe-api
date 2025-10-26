using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class SlotConfiguration : IEntityTypeConfiguration<Slot>
{
    public void Configure(EntityTypeBuilder<Slot> builder)
    {
        var storageComparer = new ValueComparer<List<string>>(
                                (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                                 c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                                 c => c.ToList()
                             );

        builder.HasOne(s => s.Service)
           .WithMany(x => x.Slots)
           .HasForeignKey(s => s.ServiceId)
           .IsRequired(false)
           .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(s => s.Task)
           .WithMany(x => x.Slots)
            .HasForeignKey(s => s.TaskId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Area)
            .WithMany(x => x.Slots)
            .HasForeignKey(s => s.AreaId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Team)
            .WithMany(x => x.Slots)
            .HasForeignKey(s => s.TeamId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.PetGroup)
            .WithMany(x => x.Slots)
            .HasForeignKey(s => s.PetGroupId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(s => s.Pet)
            .WithMany(x => x.Slots)
            .HasForeignKey(s => s.PetId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

