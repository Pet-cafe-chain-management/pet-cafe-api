
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class DailyScheduleConfiguration : IEntityTypeConfiguration<DailySchedule>
{
    public void Configure(EntityTypeBuilder<DailySchedule> builder)
    {
        builder.HasOne(x => x.TeamMember)
            .WithMany(x => x.DailySchedules)
            .HasForeignKey(x => x.TeamMemberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Employee)
            .WithMany(x => x.DailySchedules)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.WorkShift)
            .WithMany(x => x.DailySchedules)
            .HasForeignKey(x => x.WorkShiftId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}