using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetCafe.Domain.Entities;

namespace PetCafe.Infrastructures.EntityConfigurations;

public class PetSpecificConfiguration : IEntityTypeConfiguration<PetSpecies>
{
    public void Configure(EntityTypeBuilder<PetSpecies> builder)
    {
    }
}