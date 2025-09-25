using PetCafe.Application.Repositories;
using PetCafe.Application.Services.Commons;
using PetCafe.Domain.Entities;
using Task = PetCafe.Domain.Entities.Task;

namespace PetCafe.Infrastructures.Repositories;

// Account & User Management

public class AccountRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<Account>(context, currentTime, claimsService), IAccountRepository
{
}

public class CustomerRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<Customer>(context, currentTime, claimsService), ICustomerRepository
{
}

public class EmployeeRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<Employee>(context, currentTime, claimsService), IEmployeeRepository
{
}

// Pet Management
public class PetBreedRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<PetBreed>(context, currentTime, claimsService), IPetBreedRepository
{
}

public class PetGroupRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<PetGroup>(context, currentTime, claimsService), IPetGroupRepository
{
}
public class TransactionRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<Transaction>(context, currentTime, claimsService), ITransactionRepository
{
}

public class PetRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<Pet>(context, currentTime, claimsService), IPetRepository
{
}

public class HealthRecordRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<HealthRecord>(context, currentTime, claimsService), IHealthRecordRepository
{
}

public class VaccineTypeRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<VaccineType>(context, currentTime, claimsService), IVaccineTypeRepository
{
}

public class VaccinationRecordRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<VaccinationRecord>(context, currentTime, claimsService), IVaccinationRecordRepository
{
}

public class VaccinationScheduleRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<VaccinationSchedule>(context, currentTime, claimsService), IVaccinationScheduleRepository
{
}

public class PetSpeciesRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<PetSpecies>(context, currentTime, claimsService), IPetSpeciesRepository
{
}

// Team & Task Management
public class TeamRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<Team>(context, currentTime, claimsService), ITeamRepository
{
}

public class TeamMemberRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<TeamMember>(context, currentTime, claimsService), ITeamMemberRepository
{
}

public class TaskRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<Task>(context, currentTime, claimsService), ITaskRepository
{
}

public class TaskAssignmentRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<TaskAssignment>(context, currentTime, claimsService), ITaskAssignmentRepository
{
}

public class WorkShiftRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<WorkShift>(context, currentTime, claimsService), IWorkShiftRepository
{
}

public class EmployeeScheduleRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<EmployeeSchedule>(context, currentTime, claimsService), IEmployeeScheduleRepository
{
}

// Product & Sales Management
public class ProductCategoryRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<ProductCategory>(context, currentTime, claimsService), IProductCategoryRepository
{
}

public class ProductRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<Product>(context, currentTime, claimsService), IProductRepository
{
}

public class OrderRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<Order>(context, currentTime, claimsService), IOrderRepository
{
}

public class OrderDetailRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<OrderDetail>(context, currentTime, claimsService), IOrderDetailRepository
{
}

public class CustomerBookingRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<CustomerBooking>(context, currentTime, claimsService), ICustomerBookingRepository
{
}
public class SlotRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<Slot>(context, currentTime, claimsService), ISlotRepository
{
}

// Area & Service Management
public class AreaRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<Area>(context, currentTime, claimsService), IAreaRepository
{
}

public class ServiceRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<Service>(context, currentTime, claimsService), IServiceRepository
{
}

public class AreaServiceRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<AreaService>(context, currentTime, claimsService), IAreaServiceRepository
{
}


// Notification System
public class NotificationRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<Notification>(context, currentTime, claimsService), INotificationRepository
{
}

public class StorageRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<Storage>(context, currentTime, claimsService), IStorageRepository
{
}
