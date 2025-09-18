using Microsoft.EntityFrameworkCore.Storage;
using PetCafe.Application;
using PetCafe.Application.Repositories;

namespace PetCafe.Infrastructures;

public class UnitOfWork(
    AppDbContext _appDbContext,
    IAccountRepository accountRepository,
    ICustomerRepository customerRepository,
    IEmployeeRepository employeeRepository,
    IPetBreedRepository petBreedRepository,
    IPetStatusRepository petStatusRepository,
    IPetRepository petRepository,
    IHealthRecordRepository healthRecordRepository,
    IVaccineTypeRepository vaccineTypeRepository,
    IVaccinationRecordRepository vaccinationRecordRepository,
    IVaccinationScheduleRepository vaccinationScheduleRepository,
    IPetSpecificRepository petSpecificRepository,
    ITeamRepository teamRepository,
    ITeamMemberRepository teamMemberRepository,
    ITaskRepository taskRepository,
    ITaskAssignmentRepository taskAssignmentRepository,
    IWorkShiftRepository workShiftRepository,
    IEmployeeScheduleRepository employeeScheduleRepository,
    IProductCategoryRepository productCategoryRepository,
    IProductRepository productRepository,
    IOrderRepository orderRepository,
    IOrderDetailRepository orderDetailRepository,
    IStaffAssignmentRepository staffAssignmentRepository,
    IAreaRepository areaRepository,
    IServiceRepository serviceRepository,
    IAreaServiceRepository areaServiceRepository,
    IServiceBookingRepository serviceBookingRepository,
    IBookingPetRepository bookingPetRepository,
    INotificationRepository notificationRepository
) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;

    // Account & User Management
    public IAccountRepository AccountRepository => accountRepository;
    public ICustomerRepository CustomerRepository => customerRepository;
    public IEmployeeRepository EmployeeRepository => employeeRepository;

    // Pet Management
    public IPetBreedRepository PetBreedRepository => petBreedRepository;
    public IPetStatusRepository PetStatusRepository => petStatusRepository;
    public IPetRepository PetRepository => petRepository;
    public IHealthRecordRepository HealthRecordRepository => healthRecordRepository;
    public IVaccineTypeRepository VaccineTypeRepository => vaccineTypeRepository;
    public IVaccinationRecordRepository VaccinationRecordRepository => vaccinationRecordRepository;
    public IVaccinationScheduleRepository VaccinationScheduleRepository => vaccinationScheduleRepository;
    public IPetSpecificRepository PetSpecificRepository => petSpecificRepository;

    // Team & Task Management
    public ITeamRepository TeamRepository => teamRepository;
    public ITeamMemberRepository TeamMemberRepository => teamMemberRepository;
    public ITaskRepository TaskRepository => taskRepository;
    public ITaskAssignmentRepository TaskAssignmentRepository => taskAssignmentRepository;
    public IWorkShiftRepository WorkShiftRepository => workShiftRepository;
    public IEmployeeScheduleRepository EmployeeScheduleRepository => employeeScheduleRepository;

    // Product & Sales Management
    public IProductCategoryRepository ProductCategoryRepository => productCategoryRepository;
    public IProductRepository ProductRepository => productRepository;
    public IOrderRepository OrderRepository => orderRepository;
    public IOrderDetailRepository OrderDetailRepository => orderDetailRepository;
    public IStaffAssignmentRepository StaffAssignmentRepository => staffAssignmentRepository;

    // Area & Service Management
    public IAreaRepository AreaRepository => areaRepository;
    public IServiceRepository ServiceRepository => serviceRepository;
    public IAreaServiceRepository AreaServiceRepository => areaServiceRepository;
    public IServiceBookingRepository ServiceBookingRepository => serviceBookingRepository;
    public IBookingPetRepository BookingPetRepository => bookingPetRepository;

    // Notification System
    public INotificationRepository NotificationRepository => notificationRepository;

    // Transaction methods
    public async Task<bool> SaveChangesAsync()
    {
        return await _appDbContext.SaveChangesAsync() > 0;
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _appDbContext.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    // Dispose pattern
    public void Dispose()
    {
        _transaction?.Dispose();
        _appDbContext.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
        }
        await _appDbContext.DisposeAsync();
    }
}
