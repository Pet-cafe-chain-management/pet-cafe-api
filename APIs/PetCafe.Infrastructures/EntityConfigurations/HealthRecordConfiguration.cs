using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class HealthRecordConfiguration : IEntityTypeConfiguration<HealthRecord>
{
    public void Configure(EntityTypeBuilder<HealthRecord> builder)
    {

        builder.HasOne(x => x.Pet)
            .WithMany(x => x.HealthRecords)
            .HasForeignKey(x => x.PetId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}