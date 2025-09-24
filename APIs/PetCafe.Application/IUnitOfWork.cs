using PetCafe.Application.Repositories;

namespace PetCafe.Application;

public interface IUnitOfWork
{
    // Account & User Management
    IAccountRepository AccountRepository { get; }
    ICustomerRepository CustomerRepository { get; }
    IEmployeeRepository EmployeeRepository { get; }

    // Pet Management
    IPetBreedRepository PetBreedRepository { get; }
    IPetRepository PetRepository { get; }
    IPetGroupRepository PetGroupRepository { get; }
    IHealthRecordRepository HealthRecordRepository { get; }
    IVaccineTypeRepository VaccineTypeRepository { get; }
    IVaccinationRecordRepository VaccinationRecordRepository { get; }
    IVaccinationScheduleRepository VaccinationScheduleRepository { get; }
    IPetSpeciesRepository PetSpecificRepository { get; }

    // Team & Task Management
    ITeamRepository TeamRepository { get; }
    ITeamMemberRepository TeamMemberRepository { get; }
    ITaskRepository TaskRepository { get; }
    ITaskAssignmentRepository TaskAssignmentRepository { get; }
    IWorkShiftRepository WorkShiftRepository { get; }
    IEmployeeScheduleRepository EmployeeScheduleRepository { get; }

    // Product & Sales Management
    IProductCategoryRepository ProductCategoryRepository { get; }
    IProductRepository ProductRepository { get; }
    IOrderRepository OrderRepository { get; }
    IOrderDetailRepository OrderDetailRepository { get; }
    ITransactionRepository TransactionRepository { get; }
    ISlotRepository SlotRepository { get; }

    // Area & Service Management
    IAreaRepository AreaRepository { get; }
    IServiceRepository ServiceRepository { get; }
    IAreaServiceRepository AreaServiceRepository { get; }
    ICustomerBookingRepository BookingRepository { get; }

    // Notification System
    INotificationRepository NotificationRepository { get; }

    // Transaction methods
    Task<bool> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
