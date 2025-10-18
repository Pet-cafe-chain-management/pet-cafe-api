using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task = PetCafe.Domain.Entities.Task;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class TaskConfiguration : IEntityTypeConfiguration<Task>
{
    public void Configure(EntityTypeBuilder<Task> builder)
    {
        builder.HasIndex(x => new { x.TeamId, x.Status });
        builder.HasIndex(x => x.DueDate);

        builder.HasOne(x => x.Team)
            .WithMany(x => x.Tasks)
            .HasForeignKey(x => x.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Area)
            .WithMany(x => x.Tasks)
            .HasForeignKey(x => x.AreaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PetGroup)
            .WithMany(x => x.Tasks)
            .HasForeignKey(x => x.PetGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.WorkShift)
            .WithMany(x => x.Tasks)
            .HasForeignKey(x => x.WorkShiftId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}