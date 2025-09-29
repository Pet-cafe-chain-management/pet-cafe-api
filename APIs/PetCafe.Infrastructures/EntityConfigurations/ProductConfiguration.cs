using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

// ProductConfiguration.cs
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
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
        builder.HasIndex(x => new { x.CategoryId, x.IsActive });

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.IsForPets)
            .HasDefaultValue(false);
    }
}



