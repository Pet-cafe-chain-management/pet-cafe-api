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
    IPetRepository petRepository,
    IHealthRecordRepository healthRecordRepository,
    IVaccineTypeRepository vaccineTypeRepository,
    IVaccinationRecordRepository vaccinationRecordRepository,
    IVaccinationScheduleRepository vaccinationScheduleRepository,
    IPetSpeciesRepository petSpecificRepository,
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
    ICustomerBookingRepository customerBookingRepository,
    IAreaRepository areaRepository,
    IServiceRepository serviceRepository,
    IAreaServiceRepository areaServiceRepository,
    IPetGroupRepository petGroupRepository,
    INotificationRepository notificationRepository,
    ITransactionRepository transactionRepository,
    ISlotRepository slotRepository
) : IUnitOfWork
{
    private IDbContextTransaction? _transaction;

    // Account & User Management
    public IAccountRepository AccountRepository => accountRepository;
    public ICustomerRepository CustomerRepository => customerRepository;
    public IEmployeeRepository EmployeeRepository => employeeRepository;

    // Pet Management
    public IPetBreedRepository PetBreedRepository => petBreedRepository;
    public ICustomerBookingRepository BookingRepository => customerBookingRepository;
    public IPetRepository PetRepository => petRepository;
    public IHealthRecordRepository HealthRecordRepository => healthRecordRepository;
    public IVaccineTypeRepository VaccineTypeRepository => vaccineTypeRepository;
    public IVaccinationRecordRepository VaccinationRecordRepository => vaccinationRecordRepository;
    public IVaccinationScheduleRepository VaccinationScheduleRepository => vaccinationScheduleRepository;
    public IPetSpeciesRepository PetSpecificRepository => petSpecificRepository;

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
    public ITransactionRepository TransactionRepository => transactionRepository;

    // Area & Service Management
    public IAreaRepository AreaRepository => areaRepository;
    public IServiceRepository ServiceRepository => serviceRepository;
    public IAreaServiceRepository AreaServiceRepository => areaServiceRepository;
    public IPetGroupRepository PetGroupRepository => petGroupRepository;

    // Notification System
    public INotificationRepository NotificationRepository => notificationRepository;


    public ISlotRepository SlotRepository => slotRepository;

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
