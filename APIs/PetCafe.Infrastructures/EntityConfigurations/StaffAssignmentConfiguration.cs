using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class StaffAssignmentConfiguration : IEntityTypeConfiguration<StaffAssignment>
{
    public void Configure(EntityTypeBuilder<StaffAssignment> builder)
    {
        builder.HasIndex(x => new { x.OrderId, x.EmployeeId, x.AssignmentType });
        builder.HasIndex(x => new { x.EmployeeId, x.AssignedDate });

        builder.HasOne(x => x.Order)
            .WithMany(x => x.StaffAssignments)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Employee)
            .WithMany(x => x.StaffAssignments)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Status)
            .HasDefaultValue("Assigned");

        builder.Property(x => x.AssignedDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
