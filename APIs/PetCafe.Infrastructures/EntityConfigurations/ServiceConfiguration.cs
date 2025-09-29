using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        var storageComparer = new ValueComparer<List<string>>(
                                 (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                                 c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                                 c => c.ToList()
                             );
        builder.Property(b => b.Thumbnails)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new()
                )
                .HasColumnType("json")
                .Metadata.SetValueComparer(storageComparer);

        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => new { x.ServiceType, x.IsActive });


        builder.Property(x => x.MaxParticipants)
            .HasDefaultValue(1);

        builder.Property(x => x.RequiresArea)
            .HasDefaultValue(true);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);
    }
}
