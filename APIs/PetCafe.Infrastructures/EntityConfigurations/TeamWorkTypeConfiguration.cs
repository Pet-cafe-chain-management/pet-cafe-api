using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class TeamWorkTypeConfiguration : IEntityTypeConfiguration<TeamWorkType>
{
    public void Configure(EntityTypeBuilder<TeamWorkType> builder)
    {
        builder.HasOne(x => x.WorkType)
                   .WithMany(x => x.TeamWorkTypes)
                   .HasForeignKey(x => x.WorkTypeId)
                   .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Team)
                   .WithMany(x => x.TeamWorkTypes)
                   .HasForeignKey(x => x.TeamId)
                   .OnDelete(DeleteBehavior.Cascade);

    }
}