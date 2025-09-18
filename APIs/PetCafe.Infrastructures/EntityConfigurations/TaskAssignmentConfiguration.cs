using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class TaskAssignmentConfiguration : IEntityTypeConfiguration<TaskAssignment>
{
    public void Configure(EntityTypeBuilder<TaskAssignment> builder)
    {
        builder.HasIndex(x => new { x.TaskId, x.EmployeeId }).IsUnique();
        builder.HasIndex(x => new { x.EmployeeId, x.Status });

        builder.HasOne(x => x.Task)
            .WithMany(x => x.TaskAssignments)
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Employee)
            .WithMany(x => x.TaskAssignments)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Status)
            .HasDefaultValue("Assigned");

        builder.Property(x => x.AssignedDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}