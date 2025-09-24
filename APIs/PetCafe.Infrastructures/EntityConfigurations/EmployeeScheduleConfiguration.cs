using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class EmployeeScheduleConfiguration : IEntityTypeConfiguration<EmployeeSchedule>
{
    public void Configure(EntityTypeBuilder<EmployeeSchedule> builder)
    {
        builder.HasIndex(x => new { x.EmployeeId, x.WorkDate }).IsUnique();
        builder.HasIndex(x => x.WorkDate);

        builder.HasOne(x => x.Employee)
            .WithMany(x => x.Schedules)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.WorkShift)
            .WithMany(x => x.Schedules)
            .HasForeignKey(x => x.WorkShiftId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Status)
            .HasDefaultValue("Scheduled");
    }
}