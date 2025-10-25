using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class AreaWorkTypeConfiguration : IEntityTypeConfiguration<AreaWorkType>
{
    public void Configure(EntityTypeBuilder<AreaWorkType> builder)
    {
        builder.HasOne(x => x.WorkType)
                   .WithMany(x => x.AreaWorkTypes)
                   .HasForeignKey(x => x.WorkTypeId)
                   .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Area)
                   .WithMany(x => x.AreaWorkTypes)
                   .HasForeignKey(x => x.AreaId)
                   .OnDelete(DeleteBehavior.Cascade);

    }
}