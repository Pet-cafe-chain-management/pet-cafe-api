using Microsoft.EntityFrameworkCore;

namespace PetCafe.Infrastructures;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
}