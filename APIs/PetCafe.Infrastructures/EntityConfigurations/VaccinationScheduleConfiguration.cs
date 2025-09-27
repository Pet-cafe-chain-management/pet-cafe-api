using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class VaccinationScheduleConfiguration : IEntityTypeConfiguration<VaccinationSchedule>
{
    public void Configure(EntityTypeBuilder<VaccinationSchedule> builder)
    {
        builder.HasIndex(x => new { x.PetId, x.ScheduledDate });
        builder.HasIndex(x => x.ScheduledDate);

        builder.HasOne(x => x.Pet)
            .WithMany(x => x.VaccinationSchedules)
            .HasForeignKey(x => x.PetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.VaccineType)
            .WithMany(x => x.VaccinationSchedules)
            .HasForeignKey(x => x.VaccineTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Record)
            .WithOne(x => x.Schedule)
            .HasForeignKey<VaccinationSchedule>(x => x.RecordId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
