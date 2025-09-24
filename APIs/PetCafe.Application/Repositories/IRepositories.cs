using PetCafe.Domain.Entities;
using Task = PetCafe.Domain.Entities.Task;

namespace PetCafe.Application.Repositories;

// Account & User Management
public interface IAccountRepository : IGenericRepository<Account>
{
}

public interface ICustomerRepository : IGenericRepository<Customer>
{
}

public interface IEmployeeRepository : IGenericRepository<Employee>
{
}

// Pet Management
public interface IPetBreedRepository : IGenericRepository<PetBreed>
{
}


public interface IPetRepository : IGenericRepository<Pet>
{
}

public interface IPetGroupRepository : IGenericRepository<PetGroup>
{
}

public interface IHealthRecordRepository : IGenericRepository<HealthRecord>
{
}

public interface IVaccineTypeRepository : IGenericRepository<VaccineType>
{
}

public interface IVaccinationRecordRepository : IGenericRepository<VaccinationRecord>
{
}

public interface IVaccinationScheduleRepository : IGenericRepository<VaccinationSchedule>
{
}

public interface IPetSpeciesRepository : IGenericRepository<PetSpecies>
{
}

// Team & Task Management
public interface ITeamRepository : IGenericRepository<Team>
{
}

public interface ITeamMemberRepository : IGenericRepository<TeamMember>
{
}

public interface ITaskRepository : IGenericRepository<Task>
{
}

public interface ITaskAssignmentRepository : IGenericRepository<TaskAssignment>
{
}

public interface IWorkShiftRepository : IGenericRepository<WorkShift>
{
}

public interface IEmployeeScheduleRepository : IGenericRepository<EmployeeSchedule>
{
}

// Product & Sales Management
public interface IProductCategoryRepository : IGenericRepository<ProductCategory>
{
}

public interface IProductRepository : IGenericRepository<Product>
{
}

public interface IOrderRepository : IGenericRepository<Order>
{
}

public interface IOrderDetailRepository : IGenericRepository<OrderDetail>
{
}

public interface ITransactionRepository : IGenericRepository<Transaction>
{
}

public interface ISlotRepository : IGenericRepository<Slot>
{
}

// Area & Service Management
public interface IAreaRepository : IGenericRepository<Area>
{
}

public interface IServiceRepository : IGenericRepository<Service>
{
}

public interface IAreaServiceRepository : IGenericRepository<AreaService>
{
}

public interface ICustomerBookingRepository : IGenericRepository<CustomerBooking>
{
}

// Notification System
public interface INotificationRepository : IGenericRepository<Notification>
{
}

