using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.Username).IsUnique();
        builder.HasIndex(x => new { x.Role, x.IsActive });

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Ignore(x => x.Customer);
        builder.Ignore(x => x.Employee);
    }
}