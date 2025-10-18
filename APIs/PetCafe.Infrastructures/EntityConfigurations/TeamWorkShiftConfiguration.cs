using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class TeamWorkShiftConfiguration : IEntityTypeConfiguration<TeamWorkShift>
{
    public void Configure(EntityTypeBuilder<TeamWorkShift> builder)
    {

        builder.HasOne(x => x.Team)
            .WithMany(x => x.TeamWorkShifts)
            .HasForeignKey(x => x.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.WorkShift)
            .WithMany(x => x.TeamWorkShifts)
            .HasForeignKey(x => x.WorkShiftId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}