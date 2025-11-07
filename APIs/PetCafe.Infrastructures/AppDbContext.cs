using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PetCafe.Domain.Entities;
using Task = PetCafe.Domain.Entities.Task;

namespace PetCafe.Infrastructures;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // Account & User Management
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Employee> Employees { get; set; }

    // Pet Management
    public DbSet<PetBreed> PetBreeds { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<PetGroup> PetGroups { get; set; }
    public DbSet<HealthRecord> HealthRecords { get; set; }
    public DbSet<VaccineType> VaccineTypes { get; set; }
    public DbSet<VaccinationRecord> VaccinationRecords { get; set; }
    public DbSet<VaccinationSchedule> VaccinationSchedules { get; set; }
    public DbSet<PetSpecies> PetSpecies { get; set; }

    // Team & Task Management
    public DbSet<Team> Teams { get; set; }
    public DbSet<TeamMember> TeamMembers { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<WorkShift> WorkShifts { get; set; }
    // Product & Sales Management
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<ProductOrderDetail> ProductOrderDetails { get; set; }
    public DbSet<ServiceOrderDetail> ServiceOrderDetails { get; set; }
    public DbSet<ProductOrder> ProductOrders { get; set; }
    public DbSet<ServiceOrder> ServiceOrders { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Slot> Slots { get; set; }
    public DbSet<SlotAvailability> SlotAvailabilities { get; set; }

    // Area & Service Management
    public DbSet<Area> Areas { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<WorkType> WorkTypes { get; set; }
    public DbSet<AreaWorkType> AreaWorkTypes { get; set; }
    public DbSet<TeamWorkType> TeamWorkTypes { get; set; }
    public DbSet<DailyTask> DailyTasks { get; set; }
    public DbSet<TeamWorkShift> TeamWorkShifts { get; set; }
    public DbSet<CustomerBooking> ServiceBookings { get; set; }
    public DbSet<DailySchedule> DailySchedules { get; set; }
    public DbSet<ServiceFeedback> ServiceFeedbacks { get; set; }
    // Notification System
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Storage> Storages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(assembly: Assembly.GetExecutingAssembly());

        // Additional global configurations
        ConfigureGlobalSettings(modelBuilder);
    }

    private static void ConfigureGlobalSettings(ModelBuilder modelBuilder)
    {
        // Configure soft delete globally for entities inheriting from BaseEntity
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(GetSoftDeleteFilter(entityType.ClrType));
            }
        }

        // Configure naming convention for PostgreSQL (snake_case)
        ConfigureNamingConvention(modelBuilder);
    }

    private static LambdaExpression GetSoftDeleteFilter(Type entityType)
    {
        var parameter = Expression.Parameter(entityType, "e");
        var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
        var condition = Expression.Equal(property, Expression.Constant(false));
        return Expression.Lambda(condition, parameter);
    }

    private static void ConfigureNamingConvention(ModelBuilder modelBuilder)
    {
        // Convert all table names to snake_case
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (!string.IsNullOrEmpty(tableName))
            {
                entity.SetTableName(ToSnakeCase(tableName));
            }

            // Convert all column names to snake_case
            foreach (var property in entity.GetProperties())
            {
                var columnName = property.GetColumnName();
                if (!string.IsNullOrEmpty(columnName))
                {
                    property.SetColumnName(ToSnakeCase(columnName));
                }
            }

            // Convert all foreign key names to snake_case
            foreach (var key in entity.GetForeignKeys())
            {
                var constraintName = key.GetConstraintName();
                if (!string.IsNullOrEmpty(constraintName))
                {
                    key.SetConstraintName(ToSnakeCase(constraintName));
                }
            }

            // Convert all index names to snake_case
            foreach (var index in entity.GetIndexes())
            {
                var indexName = index.GetDatabaseName();
                if (!string.IsNullOrEmpty(indexName))
                {
                    index.SetDatabaseName(ToSnakeCase(indexName));
                }
            }
        }
    }

    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = new StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]) && i > 0)
            {
                result.Append('_');
            }
            result.Append(char.ToLower(input[i]));
        }
        return result.ToString();
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.IsDeleted = false;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    // Prevent modification of CreatedAt
                    entry.Property(x => x.CreatedAt).IsModified = false;
                    break;

                case EntityState.Deleted:
                    // Implement soft delete
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
    }
}
