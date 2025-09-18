using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class PetStatusConfiguration : IEntityTypeConfiguration<PetStatus>
{
    public void Configure(EntityTypeBuilder<PetStatus> builder)
    {
        builder.HasIndex(x => x.Name).IsUnique();
    }
}
