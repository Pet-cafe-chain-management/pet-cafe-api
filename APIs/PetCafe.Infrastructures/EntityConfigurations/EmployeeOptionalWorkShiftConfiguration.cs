using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class EmployeeOptionalWorkShiftConfiguration : IEntityTypeConfiguration<EmployeeOptionalWorkShift>
{
    public void Configure(EntityTypeBuilder<EmployeeOptionalWorkShift> builder)
    {
        builder.HasIndex(x => new { x.EmployeeId, x.WorkShiftId }).IsUnique();

        builder.HasOne(x => x.Employee)
            .WithMany(x => x.OptionalWorkShifts)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.WorkShift)
            .WithMany()
            .HasForeignKey(x => x.WorkShiftId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

