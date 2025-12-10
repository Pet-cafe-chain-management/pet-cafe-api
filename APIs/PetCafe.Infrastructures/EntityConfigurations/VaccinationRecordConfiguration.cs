using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class VaccinationRecordConfiguration : IEntityTypeConfiguration<VaccinationRecord>
{
    public void Configure(EntityTypeBuilder<VaccinationRecord> builder)
    {

        builder.HasOne(x => x.Pet)
            .WithMany(x => x.VaccinationRecords)
            .HasForeignKey(x => x.PetId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}