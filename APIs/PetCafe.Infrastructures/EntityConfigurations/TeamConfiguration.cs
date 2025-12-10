using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {

        builder.HasOne(x => x.Leader)
            .WithMany()
            .HasForeignKey(x => x.LeaderId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);
    }
}

