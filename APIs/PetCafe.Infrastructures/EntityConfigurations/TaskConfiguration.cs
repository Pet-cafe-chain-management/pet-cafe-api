using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task = PetCafe.Domain.Entities.Task;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class TaskConfiguration : IEntityTypeConfiguration<Task>
{
    public void Configure(EntityTypeBuilder<Task> builder)
    {
        builder.HasIndex(x => new { x.Status });

        builder.HasOne(x => x.WorkType)
            .WithMany(x => x.Tasks)
            .HasForeignKey(x => x.WorkTypeId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}