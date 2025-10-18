using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class DailyTaskConfiguration : IEntityTypeConfiguration<DailyTask>
{
    public void Configure(EntityTypeBuilder<DailyTask> builder)
    {

        builder.HasOne(x => x.Team)
            .WithMany(x => x.DailyTasks)
            .HasForeignKey(x => x.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Task)
            .WithMany(x => x.DailyTasks)
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}