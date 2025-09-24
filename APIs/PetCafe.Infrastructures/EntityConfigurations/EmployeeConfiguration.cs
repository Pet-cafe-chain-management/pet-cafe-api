using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasIndex(x => x.Phone).IsUnique();

        builder.HasOne(x => x.Account)
            .WithOne(x => x.Employee)
            .HasForeignKey<Employee>(x => x.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}