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

public interface IProductOrderDetailRepository : IGenericRepository<ProductOrderDetail>
{
}
public interface IServiceOrderDetailRepository : IGenericRepository<ServiceOrderDetail>
{
}
public interface IProductOrderRepository : IGenericRepository<ProductOrder>
{
}
public interface IServiceOrderRepository : IGenericRepository<ServiceOrder>
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


public interface ICustomerBookingRepository : IGenericRepository<CustomerBooking>
{
}

// Notification System
public interface INotificationRepository : IGenericRepository<Notification>
{
}

public interface IStorageRepository : IGenericRepository<Storage>
{
}

public interface IWorkTypeRepository : IGenericRepository<WorkType>
{
}

public interface IDailyTaskRepository : IGenericRepository<DailyTask>
{
}
public interface ITeamWorkShiftRepository : IGenericRepository<TeamWorkShift>
{
}
public interface IAreaWorkTypeRepository : IGenericRepository<AreaWorkType>
{
}
public interface ITeamWorkTypeRepository : IGenericRepository<TeamWorkType>
{
}
