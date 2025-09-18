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
    IPetStatusRepository PetStatusRepository { get; }
    IPetRepository PetRepository { get; }
    IHealthRecordRepository HealthRecordRepository { get; }
    IVaccineTypeRepository VaccineTypeRepository { get; }
    IVaccinationRecordRepository VaccinationRecordRepository { get; }
    IVaccinationScheduleRepository VaccinationScheduleRepository { get; }
    IPetSpecificRepository PetSpecificRepository { get; }

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
    IStaffAssignmentRepository StaffAssignmentRepository { get; }

    // Area & Service Management
    IAreaRepository AreaRepository { get; }
    IServiceRepository ServiceRepository { get; }
    IAreaServiceRepository AreaServiceRepository { get; }
    IServiceBookingRepository ServiceBookingRepository { get; }
    IBookingPetRepository BookingPetRepository { get; }

    // Notification System
    INotificationRepository NotificationRepository { get; }

    // Transaction methods
    Task<bool> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
