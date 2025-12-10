using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class WorkShiftConfiguration : IEntityTypeConfiguration<WorkShift>
{
    public void Configure(EntityTypeBuilder<WorkShift> builder)
    {
        var storageComparer = new ValueComparer<List<string>>(
                                       (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                                        c => c.ToList()
                                    );
        builder.Property(b => b.ApplicableDays)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new()
                )
                .HasColumnType("json")
                .Metadata.SetValueComparer(storageComparer);


    }
}