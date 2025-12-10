using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        var storageComparer = new ValueComparer<List<string>>(
                 (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                 c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                 c => c.ToList()
             );
        builder.Property(b => b.Skills)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new()
                )
                .HasColumnType("json")
                .Metadata.SetValueComparer(storageComparer);


        builder.HasOne(x => x.Account)
            .WithOne(x => x.Employee)
            .HasForeignKey<Employee>(x => x.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}