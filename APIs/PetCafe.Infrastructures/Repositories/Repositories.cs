using Microsoft.EntityFrameworkCore;
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


public class WorkShiftRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<WorkShift>(context, currentTime, claimsService), IWorkShiftRepository
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

public class ProductOrderDetailRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<ProductOrderDetail>(context, currentTime, claimsService), IProductOrderDetailRepository
{
}
public class ServiceOrderDetailRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<ServiceOrderDetail>(context, currentTime, claimsService), IServiceOrderDetailRepository
{
}
public class ServiceOrderRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<ServiceOrder>(context, currentTime, claimsService), IServiceOrderRepository
{
}
public class ProductOrderRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<ProductOrder>(context, currentTime, claimsService), IProductOrderRepository
{
}

public class CustomerBookingRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<CustomerBooking>(context, currentTime, claimsService), ICustomerBookingRepository
{
}
public class SlotRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<Slot>(context, currentTime, claimsService), ISlotRepository
{
}

public class SlotAvailabilityRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<SlotAvailability>(context, currentTime, claimsService), ISlotAvailabilityRepository
{
    public async System.Threading.Tasks.Task<SlotAvailability?> GetBySlotIdAndDateAsync(Guid slotId, DateOnly bookingDate, CancellationToken cancellationToken = default)
    {
        return await FirstOrDefaultAsync(
            x => x.SlotId == slotId && x.BookingDate == bookingDate && !x.IsDeleted,
            cancellationToken: cancellationToken
        );
    }

    public async System.Threading.Tasks.Task IncrementBookedCountAsync(Guid slotId, DateOnly bookingDate, CancellationToken cancellationToken = default)
    {
        var availability = await GetBySlotIdAndDateAsync(slotId, bookingDate, cancellationToken);

        if (availability == null)
        {
            // Get slot to get max capacity
            var slot = await context.Set<Slot>().FirstOrDefaultAsync(s => s.Id == slotId && !s.IsDeleted, cancellationToken);
            if (slot == null)
                throw new Exception($"Slot with id {slotId} not found");

            availability = new SlotAvailability
            {
                SlotId = slotId,
                BookingDate = bookingDate,
                BookedCount = 1,
                MaxCapacity = slot.MaxCapacity
            };
            await AddAsync(availability, cancellationToken);
        }
        else
        {
            if (availability.BookedCount >= availability.MaxCapacity)
                throw new Exception($"Slot {slotId} on {bookingDate} is already at full capacity");

            availability.BookedCount++;
            Update(availability);
        }
    }

    public async System.Threading.Tasks.Task DecrementBookedCountAsync(Guid slotId, DateOnly bookingDate, CancellationToken cancellationToken = default)
    {
        var availability = await GetBySlotIdAndDateAsync(slotId, bookingDate, cancellationToken);

        if (availability != null && availability.BookedCount > 0)
        {
            availability.BookedCount--;
            Update(availability);
        }
    }
}

// Area & Service Management
public class AreaRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<Area>(context, currentTime, claimsService), IAreaRepository
{
}

public class ServiceRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<Service>(context, currentTime, claimsService), IServiceRepository
{
}

// Notification System
public class NotificationRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<Notification>(context, currentTime, claimsService), INotificationRepository
{
}

public class StorageRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<Storage>(context, currentTime, claimsService), IStorageRepository
{
}


public class WorkTypeRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<WorkType>(context, currentTime, claimsService), IWorkTypeRepository
{
}

public class DailyTaskRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<DailyTask>(context, currentTime, claimsService), IDailyTaskRepository
{
}

public class TeamWorkShiftRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<TeamWorkShift>(context, currentTime, claimsService), ITeamWorkShiftRepository
{
}
public class AreaWorkTypeRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<AreaWorkType>(context, currentTime, claimsService), IAreaWorkTypeRepository
{
}
public class TeamWorkTypeRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<TeamWorkType>(context, currentTime, claimsService), ITeamWorkTypeRepository
{
}

public class DailyScheduleRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<DailySchedule>(context, currentTime, claimsService), IDailyScheduleRepository
{
}

public class FeedbackRepository(AppDbContext context, ICurrentTime currentTime, IClaimsService claimsService) : GenericRepository<ServiceFeedback>(context, currentTime, claimsService), IFeedbackRepository
{
    public async Task<List<ServiceFeedback>> GetByServiceIdAsync(Guid serviceId, CancellationToken cancellationToken = default)
    {
        return await WhereAsync(
            x => x.ServiceId == serviceId,
            includeFunc: x => x
                .Include(f => f.Customer)
                .Include(f => f.Service)
                .Include(f => f.CustomerBooking),
            cancellationToken: cancellationToken
        );
    }
}