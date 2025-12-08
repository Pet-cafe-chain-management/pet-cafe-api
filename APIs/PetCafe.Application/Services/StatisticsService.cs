using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PetCafe.Application.Models.StatisticsModels;
using PetCafe.Application.Services.Commons;
using PetCafe.Domain.Constants;
using PetCafe.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace PetCafe.Application.Services;

public interface IStatisticsService
{
    // 1. DOANH THU VÀ BÁN HÀNG
    Task<RevenueStatisticsResponse> GetRevenueStatisticsAsync(TimePeriodFilter filter);
    Task<OrderStatisticsResponse> GetOrderStatisticsAsync(TimePeriodFilter filter);
    Task<ProductStatisticsResponse> GetProductStatisticsAsync(TimePeriodFilter filter);

    // 2. THỐNG KÊ DỊCH VỤ
    Task<ServiceStatisticsResponse> GetServiceStatisticsAsync(TimePeriodFilter filter);
    Task<SlotStatisticsResponse> GetSlotStatisticsAsync(DateRangeFilter filter);
    Task<FeedbackStatisticsResponse> GetFeedbackStatisticsAsync();

    // 3. THỐNG KÊ THÚ CƯNG
    Task<PetStatisticsResponse> GetPetStatisticsAsync();
    Task<PetHealthStatisticsResponse> GetPetHealthStatisticsAsync();
    Task<PetGroupStatisticsResponse> GetPetGroupStatisticsAsync();

    // 4. THỐNG KÊ NHÂN SỰ
    Task<EmployeeStatisticsResponse> GetEmployeeStatisticsAsync();
    Task<TeamStatisticsResponse> GetTeamStatisticsAsync();
    Task<EmployeePerformanceResponse> GetEmployeePerformanceAsync(TimePeriodFilter filter);

    // 5. THỐNG KÊ TASK & CÔNG VIỆC
    Task<TaskStatisticsResponse> GetTaskStatisticsAsync();
    Task<DailyTaskStatisticsResponse> GetDailyTaskStatisticsAsync(TimePeriodFilter filter);
    Task<WorkShiftStatisticsResponse> GetWorkShiftStatisticsAsync();

    // 6. THỐNG KÊ KHÁCH HÀNG
    Task<CustomerStatisticsResponse> GetCustomerStatisticsAsync(TimePeriodFilter filter);

    // 7. THỐNG KÊ KHO & VẬT TƯ
    Task<InventoryStatisticsResponse> GetInventoryStatisticsAsync();

    // 8. THỐNG KÊ TỔNG QUAN (Dashboard)
    Task<DashboardOverviewResponse> GetDashboardOverviewAsync();
}

public class StatisticsService(IUnitOfWork _unitOfWork) : IStatisticsService
{
    #region 1. DOANH THU VÀ BÁN HÀNG

    public async Task<RevenueStatisticsResponse> GetRevenueStatisticsAsync(TimePeriodFilter filter)
    {
        var (startDate, endDate) = GetDateRange(filter);
        var previousPeriod = GetPreviousPeriod(filter.StartDate, filter.EndDate, filter.Period);

        var orders = await _unitOfWork.OrderRepository.WhereAsync(
            x => x.OrderDate >= startDate && x.OrderDate <= endDate && x.PaymentStatus == PaymentStatusConstant.PAID
        );

        var previousOrders = await _unitOfWork.OrderRepository.WhereAsync(
            x => x.OrderDate >= previousPeriod.Start && x.OrderDate <= previousPeriod.End && x.PaymentStatus == PaymentStatusConstant.PAID
        );

        var totalRevenue = orders.Sum(x => x.FinalAmount);
        var previousRevenue = previousOrders.Sum(x => x.FinalAmount);
        var growthRate = previousRevenue > 0 ? ((totalRevenue - previousRevenue) / previousRevenue) * 100 : 0;

        var response = new RevenueStatisticsResponse
        {
            TotalRevenue = totalRevenue,
            PreviousPeriodRevenue = previousRevenue,
            GrowthRate = growthRate,
            AverageOrderValue = orders.Any() ? totalRevenue / orders.Count : 0,
            RevenueByPaymentMethod = orders
                .GroupBy(x => x.PaymentMethod ?? PaymentMethodConstant.ONLINE)
                .Select(g => new RevenueByPaymentMethodItem
                {
                    PaymentMethod = g.Key,
                    Revenue = g.Sum(x => x.FinalAmount)
                }).ToList(),
            RevenueByOrderStatus = orders
                .GroupBy(x => x.Status)
                .Select(g => new RevenueByOrderStatusItem
                {
                    Status = g.Key,
                    Revenue = g.Sum(x => x.FinalAmount)
                }).ToList(),
            RevenueByOrderType = orders
                .GroupBy(x => x.Type)
                .Select(g => new RevenueByOrderTypeItem
                {
                    OrderType = g.Key,
                    Revenue = g.Sum(x => x.FinalAmount)
                }).ToList(),
            RevenueByPeriod = GroupByPeriod(orders, filter.Period ?? "month")
                .Select(g => new RevenueByPeriodItem
                {
                    Period = g.Key,
                    Revenue = g.Sum(x => x.FinalAmount)
                }).ToList()
        };

        return response;
    }

    public async Task<OrderStatisticsResponse> GetOrderStatisticsAsync(TimePeriodFilter filter)
    {
        var (startDate, endDate) = GetDateRange(filter);
        var previousPeriod = GetPreviousPeriod(filter.StartDate, filter.EndDate, filter.Period);

        var orders = await _unitOfWork.OrderRepository.WhereAsync(
            x => x.OrderDate >= startDate && x.OrderDate <= endDate
        );

        var previousOrders = await _unitOfWork.OrderRepository.WhereAsync(
            x => x.OrderDate >= previousPeriod.Start && x.OrderDate <= previousPeriod.End
        );

        var totalOrders = orders.Count;
        var previousTotalOrders = previousOrders.Count;
        var growthRate = previousTotalOrders > 0 ? ((double)(totalOrders - previousTotalOrders) / previousTotalOrders) * 100 : 0;

        // Top customers by order count
        var topCustomersByOrder = orders
            .Where(x => x.CustomerId.HasValue)
            .GroupBy(x => new { x.CustomerId, x.Customer!.FullName })
            .Select(g => new TopCustomerByOrderItem
            {
                CustomerId = g.Key.CustomerId!.Value,
                CustomerName = g.Key.FullName ?? "N/A",
                OrderCount = g.Count()
            })
            .OrderByDescending(x => x.OrderCount)
            .Take(10)
            .ToList();

        // Top customers by revenue
        var topCustomersByRevenue = orders
            .Where(x => x.CustomerId.HasValue && x.PaymentStatus == PaymentStatusConstant.PAID)
            .GroupBy(x => new { x.CustomerId, x.Customer!.FullName })
            .Select(g => new TopCustomerByRevenueItem
            {
                CustomerId = g.Key.CustomerId!.Value,
                CustomerName = g.Key.FullName ?? "N/A",
                TotalRevenue = g.Sum(x => x.FinalAmount)
            })
            .OrderByDescending(x => x.TotalRevenue)
            .Take(10)
            .ToList();

        var response = new OrderStatisticsResponse
        {
            TotalOrders = totalOrders,
            PreviousPeriodOrders = previousTotalOrders,
            GrowthRate = growthRate,
            OrdersByStatus = orders
                .GroupBy(x => x.Status)
                .Select(g => new OrderByStatusItem
                {
                    Status = g.Key,
                    Count = g.Count()
                }).ToList(),
            OrdersByPeriod = GroupByPeriod(orders, filter.Period ?? "month")
                .Select(g => new OrderByPeriodItem
                {
                    Period = g.Key,
                    Count = g.Count()
                }).ToList(),
            TopCustomersByOrderCount = topCustomersByOrder,
            TopCustomersByRevenue = topCustomersByRevenue
        };

        return response;
    }

    public async Task<ProductStatisticsResponse> GetProductStatisticsAsync(TimePeriodFilter filter)
    {
        var (startDate, endDate) = GetDateRange(filter);

        var productOrderDetails = await _unitOfWork.ProductOrderDetailRepository.WhereAsync(
            x => x.CreatedAt >= startDate && x.CreatedAt <= endDate && x.ProductId.HasValue,
            includeFunc: x => x.Include(d => d.Product!).Include(d => d.ProductOrder!)
        );

        var allProducts = await _unitOfWork.ProductRepository.WhereAsync(x => x.IsActive);

        // Top selling by quantity
        var topByQuantity = productOrderDetails
            .GroupBy(x => new { x.ProductId, x.Product!.Name })
            .Select(g => new TopSellingProductItem
            {
                ProductId = g.Key.ProductId!.Value,
                ProductName = g.Key.Name ?? "N/A",
                Quantity = g.Sum(x => x.Quantity),
                Revenue = g.Sum(x => x.TotalPrice)
            })
            .OrderByDescending(x => x.Quantity)
            .Take(10)
            .ToList();

        // Top selling by revenue
        var topByRevenue = productOrderDetails
            .GroupBy(x => new { x.ProductId, x.Product!.Name })
            .Select(g => new TopSellingProductItem
            {
                ProductId = g.Key.ProductId!.Value,
                ProductName = g.Key.Name ?? "N/A",
                Quantity = g.Sum(x => x.Quantity),
                Revenue = g.Sum(x => x.TotalPrice)
            })
            .OrderByDescending(x => x.Revenue)
            .Take(10)
            .ToList();

        // Low stock products
        var lowStock = allProducts
            .Where(x => x.StockQuantity <= x.MinStockLevel)
            .Select(x => new LowStockProductItem
            {
                ProductId = x.Id,
                ProductName = x.Name,
                StockQuantity = x.StockQuantity,
                MinStockLevel = x.MinStockLevel
            })
            .ToList();

        // No sales products
        var soldProductIds = productOrderDetails.Where(x => x.ProductId.HasValue).Select(x => x.ProductId!.Value).Distinct().ToList();
        var noSales = allProducts
            .Where(x => !soldProductIds.Contains(x.Id))
            .Select(x => new NoSalesProductItem
            {
                ProductId = x.Id,
                ProductName = x.Name,
                StockQuantity = x.StockQuantity
            })
            .ToList();

        var response = new ProductStatisticsResponse
        {
            TopSellingByQuantity = topByQuantity,
            TopSellingByRevenue = topByRevenue,
            LowStockProducts = lowStock,
            NoSalesProducts = noSales,
            TotalInventoryValue = allProducts.Sum(x => x.StockQuantity * (x.Cost ?? x.Price))
        };

        return response;
    }

    #endregion

    #region 2. THỐNG KÊ DỊCH VỤ

    public async Task<ServiceStatisticsResponse> GetServiceStatisticsAsync(TimePeriodFilter filter)
    {
        var (startDate, endDate) = GetDateRange(filter);

        var bookings = await _unitOfWork.BookingRepository.WhereAsync(
            x => x.BookingDate >= startDate && x.BookingDate <= endDate,
            includeFunc: x => x.Include(b => b.Service).Include(b => b.OrderDetail)
        );

        var totalBookings = bookings.Count;
        var completedBookings = bookings.Count(x => x.BookingStatus == BookingStatusConstant.COMPLETED);
        var cancelledBookings = bookings.Count(x => x.BookingStatus == BookingStatusConstant.CANCELLED);
        var completionRate = totalBookings > 0 ? (double)completedBookings / totalBookings * 100 : 0;
        var cancellationRate = totalBookings > 0 ? (double)cancelledBookings / totalBookings * 100 : 0;

        // Top services
        var topServices = bookings
            .GroupBy(x => new { x.ServiceId, x.Service.Name })
            .Select(g => new TopServiceItem
            {
                ServiceId = g.Key.ServiceId,
                ServiceName = g.Key.Name,
                BookingCount = g.Count()
            })
            .OrderByDescending(x => x.BookingCount)
            .Take(10)
            .ToList();

        // Service revenues
        var serviceRevenues = bookings
            .Where(x => x.OrderDetail != null && x.BookingStatus == BookingStatusConstant.COMPLETED)
            .GroupBy(x => new { x.ServiceId, x.Service.Name })
            .Select(g => new ServiceRevenueItem
            {
                ServiceId = g.Key.ServiceId,
                ServiceName = g.Key.Name,
                Revenue = g.Sum(x => x.OrderDetail!.TotalPrice)
            })
            .OrderByDescending(x => x.Revenue)
            .ToList();

        var response = new ServiceStatisticsResponse
        {
            TotalBookings = totalBookings,
            CompletionRate = completionRate,
            CancellationRate = cancellationRate,
            BookingsByStatus = bookings
                .GroupBy(x => x.BookingStatus)
                .Select(g => new BookingByStatusItem
                {
                    Status = g.Key,
                    Count = g.Count()
                }).ToList(),
            TopServices = topServices,
            ServiceRevenues = serviceRevenues,
            BookingsByPeriod = GroupByPeriod(bookings, filter.Period ?? "month", x => x.BookingDate)
                .Select(g => new BookingByPeriodItem
                {
                    Period = g.Key,
                    Count = g.Count()
                }).ToList()
        };

        return response;
    }

    public async Task<SlotStatisticsResponse> GetSlotStatisticsAsync(DateRangeFilter filter)
    {
        var (startDate, endDate) = GetDateRange(filter);

        var slots = await _unitOfWork.SlotRepository.WhereAsync(
            x => x.CreatedAt >= startDate && x.CreatedAt <= endDate,
            includeFunc: x => x.Include(s => s.Area)
        );

        var bookings = await _unitOfWork.BookingRepository.WhereAsync(
            x => x.BookingDate >= startDate && x.BookingDate <= endDate && x.BookingStatus != BookingStatusConstant.CANCELLED
        );

        var totalSlots = slots.Count;
        var occupiedSlots = bookings.Select(x => x.SlotId).Distinct().Count();
        var utilizationRate = totalSlots > 0 ? (double)occupiedSlots / totalSlots * 100 : 0;

        // Slot by area
        var slotByArea = slots
            .GroupBy(x => new { x.AreaId, x.Area.Name })
            .Select(g => new SlotByAreaItem
            {
                AreaId = g.Key.AreaId,
                AreaName = g.Key.Name ?? "N/A",
                SlotCount = g.Count()
            })
            .ToList();

        // Slot availability by day
        var slotAvailability = new List<SlotAvailabilityItem>();
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            var dayBookings = bookings.Where(b => b.BookingDate.Date == date.Date).Select(b => b.SlotId).Distinct().Count();
            slotAvailability.Add(new SlotAvailabilityItem
            {
                Date = date,
                Available = totalSlots - dayBookings,
                Occupied = dayBookings,
                Total = totalSlots
            });
        }

        var response = new SlotStatisticsResponse
        {
            UtilizationRate = utilizationRate,
            SlotByArea = slotByArea,
            SlotAvailabilityByDay = slotAvailability
        };

        return response;
    }

    public async Task<FeedbackStatisticsResponse> GetFeedbackStatisticsAsync()
    {
        var feedbacks = await _unitOfWork.FeedbackRepository.WhereAsync(
            x => true,
            includeFunc: x => x.Include(f => f.Service)
        );

        var totalFeedbacks = feedbacks.Count;
        var averageRating = feedbacks.Any() ? feedbacks.Average(x => x.Rating) : 0;

        var ratingDistribution = Enumerable.Range(1, 5)
            .Select(rating => new RatingDistributionItem
            {
                Rating = rating,
                Count = feedbacks.Count(x => x.Rating == rating),
                Percentage = totalFeedbacks > 0 ? (double)feedbacks.Count(x => x.Rating == rating) / totalFeedbacks * 100 : 0
            })
            .ToList();

        var topRatedServices = feedbacks
            .GroupBy(x => new { x.ServiceId, x.Service.Name })
            .Select(g => new TopRatedServiceItem
            {
                ServiceId = g.Key.ServiceId,
                ServiceName = g.Key.Name,
                AverageRating = g.Average(x => x.Rating),
                FeedbackCount = g.Count()
            })
            .Where(x => x.FeedbackCount >= 3) // At least 3 feedbacks
            .OrderByDescending(x => x.AverageRating)
            .Take(10)
            .ToList();

        var response = new FeedbackStatisticsResponse
        {
            TotalFeedbacks = totalFeedbacks,
            AverageRating = averageRating,
            RatingDistribution = ratingDistribution,
            TopRatedServices = topRatedServices
        };

        return response;
    }

    #endregion

    #region 3. THỐNG KÊ THÚ CƯNG

    public async Task<PetStatisticsResponse> GetPetStatisticsAsync()
    {
        var pets = await _unitOfWork.PetRepository.WhereAsync(
            x => true,
            includeFunc: x => x.Include(p => p.Species).Include(p => p.Breed)
        );

        var response = new PetStatisticsResponse
        {
            TotalPets = pets.Count,
            PetsBySpecies = pets
                .GroupBy(x => new { x.SpeciesId, x.Species.Name })
                .Select(g => new PetBySpeciesItem
                {
                    SpeciesId = g.Key.SpeciesId,
                    SpeciesName = g.Key.Name ?? "N/A",
                    Count = g.Count()
                }).ToList(),
            PetsByBreed = pets
                .GroupBy(x => new { x.BreedId, x.Breed.Name })
                .Select(g => new PetByBreedItem
                {
                    BreedId = g.Key.BreedId,
                    BreedName = g.Key.Name ?? "N/A",
                    Count = g.Count()
                }).ToList(),
            PetsByGender = pets
                .GroupBy(x => x.Gender)
                .Select(g => new PetByGenderItem
                {
                    Gender = g.Key,
                    Count = g.Count()
                }).ToList(),
            PetsByAgeGroup = pets
                .Select(p => new
                {
                    Pet = p,
                    AgeGroup = GetAgeGroup(p.Age)
                })
                .GroupBy(x => x.AgeGroup)
                .Select(g => new PetByAgeGroupItem
                {
                    AgeGroup = g.Key,
                    Count = g.Count()
                }).ToList(),
            PetArrivalsByMonth = pets
                .GroupBy(x => x.ArrivalDate.ToString("yyyy-MM"))
                .Select(g => new PetArrivalByMonthItem
                {
                    Month = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Month)
                .ToList()
        };

        return response;
    }

    public async Task<PetHealthStatisticsResponse> GetPetHealthStatisticsAsync()
    {
        var pets = await _unitOfWork.PetRepository.WhereAsync(
            x => true,
            includeFunc: x => x.Include(p => p.HealthRecords).Include(p => p.VaccinationRecords).Include(p => p.VaccinationSchedules)
        );

        var petsWithHealthRecords = pets.Count(x => x.HealthRecords.Any());


        var vaccinatedCount = pets.Count(p => p.VaccinationRecords.Any());
        var notVaccinatedCount = pets.Count - vaccinatedCount;
        var vaccinationRate = pets.Any() ? (double)vaccinatedCount / pets.Count * 100 : 0;

        var healthChecksByMonth = pets
            .SelectMany(p => p.HealthRecords.Select(r => r.CreatedAt.ToString("yyyy-MM")))
            .GroupBy(x => x)
            .Select(g => new HealthCheckByMonthItem
            {
                Month = g.Key,
                Count = g.Count()
            })
            .OrderBy(x => x.Month)
            .ToList();

        var response = new PetHealthStatisticsResponse
        {
            PetsWithHealthRecords = petsWithHealthRecords,
            VaccinationStatus = new VaccinationStatusItem
            {
                VaccinatedCount = vaccinatedCount,
                NotVaccinatedCount = notVaccinatedCount,
                VaccinationRate = vaccinationRate
            },
            HealthChecksByMonth = healthChecksByMonth
        };

        return response;
    }

    public async Task<PetGroupStatisticsResponse> GetPetGroupStatisticsAsync()
    {
        var petGroups = await _unitOfWork.PetGroupRepository.WhereAsync(
            x => true,
            includeFunc: x => x.Include(g => g.Pets)
        );

        var response = new PetGroupStatisticsResponse
        {
            TotalGroups = petGroups.Count,
            PetGroupDetails = petGroups
                .Select(g => new PetGroupDetailItem
                {
                    GroupId = g.Id,
                    GroupName = g.Name ?? "N/A",
                    PetCount = g.Pets?.Count ?? 0
                })
                .ToList()
        };

        return response;
    }

    #endregion

    #region 4. THỐNG KÊ NHÂN SỰ

    public async Task<EmployeeStatisticsResponse> GetEmployeeStatisticsAsync()
    {
        var employees = await _unitOfWork.EmployeeRepository.WhereAsync(x => true);

        var response = new EmployeeStatisticsResponse
        {
            TotalEmployees = employees.Count,
            ActiveEmployees = employees.Count, // Assuming all are active unless IsDeleted
            InactiveEmployees = 0, // Would need IsActive flag
            EmployeesBySubRole = employees
                .GroupBy(x => x.SubRole)
                .Select(g => new EmployeeBySubRoleItem
                {
                    SubRole = g.Key,
                    Count = g.Count()
                }).ToList(),
            NewEmployeesByMonth = employees
                .GroupBy(x => x.CreatedAt.ToString("yyyy-MM"))
                .Select(g => new NewEmployeeByMonthItem
                {
                    Month = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Month)
                .ToList(),
            TotalSalaryCost = employees.Sum(x => x.Salary ?? 0)
        };

        return response;
    }

    public async Task<TeamStatisticsResponse> GetTeamStatisticsAsync()
    {
        var teams = await _unitOfWork.TeamRepository.WhereAsync(
            x => true,
            includeFunc: x => x.Include(t => t.TeamMembers)
        );

        var response = new TeamStatisticsResponse
        {
            TotalTeams = teams.Count,
            ActiveTeams = teams.Count(x => x.IsActive),
            InactiveTeams = teams.Count(x => !x.IsActive),
            TeamsByStatus = teams
                .GroupBy(x => x.Status)
                .Select(g => new TeamByStatusItem
                {
                    Status = g.Key,
                    Count = g.Count()
                }).ToList(),
            AverageMembersPerTeam = teams.Any() ? teams.Average(t => t.TeamMembers?.Count ?? 0) : 0
        };

        return response;
    }

    public async Task<EmployeePerformanceResponse> GetEmployeePerformanceAsync(TimePeriodFilter filter)
    {
        var (startDate, endDate) = GetDateRange(filter);

        var bookings = await _unitOfWork.BookingRepository.WhereAsync(
            x => x.BookingDate >= startDate && x.BookingDate <= endDate && x.BookingStatus == BookingStatusConstant.COMPLETED,
            includeFunc: x => x.Include(b => b.Team).ThenInclude(t => t.TeamMembers).ThenInclude(tm => tm.Employee)
        );

        // This is simplified - would need better relationship tracking
        var dailyTasks = await _unitOfWork.DailyTaskRepository.WhereAsync(
            x => x.AssignedDate >= startDate && x.AssignedDate <= endDate && x.Status == DailyTaskStatusConstant.COMPLETED
        );

        var response = new EmployeePerformanceResponse
        {
            TopPerformingEmployees = new List<TopPerformingEmployeeItem>(),
            EmployeeBookingCompletions = new List<EmployeeBookingCompletionItem>(),
            EmployeeTaskCompletions = new List<EmployeeTaskCompletionItem>()
        };

        return response;
    }

    #endregion

    #region 5. THỐNG KÊ TASK & CÔNG VIỆC

    public async Task<TaskStatisticsResponse> GetTaskStatisticsAsync()
    {
        var tasks = await _unitOfWork.TaskRepository.WhereAsync(
            x => true,
            includeFunc: x => x.Include(t => t.WorkType)
        );

        var totalTasks = tasks.Count;
        var activeTasks = tasks.Count(x => x.Status == TaskStatusConstant.ACTIVE);
        var completionRate = totalTasks > 0 ? (double)activeTasks / totalTasks * 100 : 0;

        var response = new TaskStatisticsResponse
        {
            TotalTasks = totalTasks,
            CompletionRate = completionRate,
            TasksByStatus = tasks
                .GroupBy(x => x.Status)
                .Select(g => new TaskByStatusItem
                {
                    Status = g.Key,
                    Count = g.Count()
                }).ToList(),
            TasksByPriority = tasks
                .GroupBy(x => x.Priority)
                .Select(g => new TaskByPriorityItem
                {
                    Priority = g.Key,
                    Count = g.Count()
                }).ToList(),
            TasksByWorkType = tasks
                .GroupBy(x => new { x.WorkTypeId, x.WorkType.Name })
                .Select(g => new TaskByWorkTypeItem
                {
                    WorkTypeId = g.Key.WorkTypeId,
                    WorkTypeName = g.Key.Name ?? "N/A",
                    Count = g.Count()
                }).ToList(),
            TaskPublicPrivate = new TaskPublicPrivateItem
            {
                PublicTasks = tasks.Count(x => x.IsPublic),
                PrivateTasks = tasks.Count(x => !x.IsPublic)
            }
        };

        return response;
    }

    public async Task<DailyTaskStatisticsResponse> GetDailyTaskStatisticsAsync(TimePeriodFilter filter)
    {
        var (startDate, endDate) = GetDateRange(filter);

        var dailyTasks = await _unitOfWork.DailyTaskRepository.WhereAsync(
            x => x.AssignedDate >= startDate && x.AssignedDate <= endDate,
            includeFunc: x => x.Include(d => d.Team)
        );

        var overdueTasks = dailyTasks
            .Where(x => x.Status != DailyTaskStatusConstant.COMPLETED && x.AssignedDate < DateTime.UtcNow)
            .Select(x => new OverdueTaskItem
            {
                TaskId = x.TaskId ?? Guid.Empty,
                TaskTitle = x.Title,
                AssignedDate = x.AssignedDate,
                DueDate = x.EndTime.HasValue ? x.AssignedDate.Add(x.EndTime.Value) : null,
                DaysOverdue = (DateTime.UtcNow - x.AssignedDate).Days
            })
            .OrderByDescending(x => x.DaysOverdue)
            .ToList();

        var response = new DailyTaskStatisticsResponse
        {
            DailyTasksByPeriod = GroupByPeriod(dailyTasks, filter.Period ?? "month", x => x.AssignedDate)
                .Select(g => new DailyTaskByPeriodItem
                {
                    Period = g.Key,
                    Completed = g.Count(x => x.Status == DailyTaskStatusConstant.COMPLETED),
                    Pending = g.Count(x => x.Status != DailyTaskStatusConstant.COMPLETED)
                }).ToList(),
            DailyTasksByStatus = dailyTasks
                .GroupBy(x => x.Status)
                .Select(g => new DailyTaskByStatusItem
                {
                    Status = g.Key,
                    Count = g.Count()
                }).ToList(),
            DailyTasksByTeam = dailyTasks
                .GroupBy(x => new { x.TeamId, x.Team.Name })
                .Select(g => new DailyTaskByTeamItem
                {
                    TeamId = g.Key.TeamId,
                    TeamName = g.Key.Name ?? "N/A",
                    TaskCount = g.Count()
                }).ToList(),
            OverdueTasks = overdueTasks
        };

        return response;
    }

    public async Task<WorkShiftStatisticsResponse> GetWorkShiftStatisticsAsync()
    {
        var workShifts = await _unitOfWork.WorkShiftRepository.WhereAsync(x => true);

        var response = new WorkShiftStatisticsResponse
        {
            TotalWorkShifts = workShifts.Count,
            WorkShiftAssignments = workShifts
                .Select(ws => new WorkShiftAssignmentItem
                {
                    WorkShiftId = ws.Id,
                    WorkShiftName = ws.Name ?? "N/A",
                    EmployeeCount = 0 // Would need relationship
                })
                .ToList(),
            UtilizationRate = 0 // Would need calculation
        };

        return response;
    }

    #endregion

    #region 6. THỐNG KÊ KHÁCH HÀNG

    public async Task<CustomerStatisticsResponse> GetCustomerStatisticsAsync(TimePeriodFilter filter)
    {
        var customers = await _unitOfWork.CustomerRepository.WhereAsync(x => true);

        var orders = await _unitOfWork.OrderRepository.WhereAsync(
            x => x.CustomerId.HasValue,
            includeFunc: x => x.Include(o => o.Customer!)
        );

        var response = new CustomerStatisticsResponse
        {
            TotalCustomers = customers.Count,
            ActiveCustomers = customers.Count, // Assuming all are active unless IsDeleted
            InactiveCustomers = 0,
            NewCustomersByPeriod = customers
                .GroupBy(x => x.CreatedAt.ToString("yyyy-MM"))
                .Select(g => new NewCustomerByPeriodItem
                {
                    Period = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Period)
                .ToList(),
            TopCustomersByOrderCount = orders
                .Where(x => x.CustomerId.HasValue)
                .GroupBy(x => new { x.CustomerId, x.Customer!.FullName })
                .Select(g => new TopCustomerItem
                {
                    CustomerId = g.Key.CustomerId!.Value,
                    CustomerName = g.Key.FullName ?? "N/A",
                    OrderCount = g.Count(),
                    TotalRevenue = g.Where(o => o.PaymentStatus == PaymentStatusConstant.PAID).Sum(o => o.FinalAmount),
                    LoyaltyPoints = customers.FirstOrDefault(c => c.Id == g.Key.CustomerId)?.LoyaltyPoints ?? 0
                })
                .OrderByDescending(x => x.OrderCount)
                .Take(10)
                .ToList(),
            TopCustomersByRevenue = orders
                .Where(x => x.CustomerId.HasValue && x.PaymentStatus == PaymentStatusConstant.PAID)
                .GroupBy(x => new { x.CustomerId, x.Customer!.FullName })
                .Select(g => new TopCustomerItem
                {
                    CustomerId = g.Key.CustomerId!.Value,
                    CustomerName = g.Key.FullName ?? "N/A",
                    OrderCount = g.Count(),
                    TotalRevenue = g.Sum(o => o.FinalAmount),
                    LoyaltyPoints = customers.FirstOrDefault(c => c.Id == g.Key.CustomerId)?.LoyaltyPoints ?? 0
                })
                .OrderByDescending(x => x.TotalRevenue)
                .Take(10)
                .ToList(),
            TotalLoyaltyPoints = customers.Sum(x => x.LoyaltyPoints)
        };

        return response;
    }

    #endregion

    #region 7. THỐNG KÊ KHO & VẬT TƯ

    public async Task<InventoryStatisticsResponse> GetInventoryStatisticsAsync()
    {
        var products = await _unitOfWork.ProductRepository.WhereAsync(x => x.IsActive);

        var productOrderDetails = await _unitOfWork.ProductOrderDetailRepository.WhereAsync(
            x => x.ProductId.HasValue && x.CreatedAt >= DateTime.UtcNow.AddYears(-1),
            includeFunc: x => x.Include(d => d.Product!)
        );

        var soldProductIds = productOrderDetails.Select(x => x.ProductId!.Value).Distinct().ToList();

        var lowStock = products
            .Where(x => x.StockQuantity <= x.MinStockLevel)
            .Select(x => new LowStockItem
            {
                ProductId = x.Id,
                ProductName = x.Name,
                StockQuantity = x.StockQuantity,
                MinStockLevel = x.MinStockLevel,
                Difference = x.StockQuantity - x.MinStockLevel
            })
            .ToList();

        var longStock = products
            .Where(x => !soldProductIds.Contains(x.Id))
            .Select(x => new LongStockItem
            {
                ProductId = x.Id,
                ProductName = x.Name,
                StockQuantity = x.StockQuantity,
                CreatedAt = x.CreatedAt,
                DaysInStock = (DateTime.UtcNow - x.CreatedAt).Days
            })
            .OrderByDescending(x => x.DaysInStock)
            .Take(20)
            .ToList();

        var response = new InventoryStatisticsResponse
        {
            TotalProducts = products.Count,
            TotalInventoryValue = products.Sum(x => x.StockQuantity * (x.Cost ?? x.Price)),
            LowStockProducts = lowStock,
            LongStockProducts = longStock
        };

        return response;
    }

    #endregion

    #region 8. THỐNG KÊ TỔNG QUAN (Dashboard)

    public async Task<DashboardOverviewResponse> GetDashboardOverviewAsync()
    {
        var today = DateTime.UtcNow.Date;
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var startOfYear = new DateTime(today.Year, 1, 1);

        // Revenue
        var ordersToday = await _unitOfWork.OrderRepository.WhereAsync(
            x => x.OrderDate.Date == today && x.PaymentStatus == PaymentStatusConstant.PAID);
        var ordersThisWeek = await _unitOfWork.OrderRepository.WhereAsync(
            x => x.OrderDate >= startOfWeek && x.OrderDate <= today && x.PaymentStatus == PaymentStatusConstant.PAID);
        var ordersThisMonth = await _unitOfWork.OrderRepository.WhereAsync(
            x => x.OrderDate >= startOfMonth && x.OrderDate <= today && x.PaymentStatus == PaymentStatusConstant.PAID);
        var ordersThisYear = await _unitOfWork.OrderRepository.WhereAsync(
            x => x.OrderDate >= startOfYear && x.OrderDate <= today && x.PaymentStatus == PaymentStatusConstant.PAID);

        var revenue = new DashboardRevenueItem
        {
            Today = ordersToday.Sum(x => x.FinalAmount),
            ThisWeek = ordersThisWeek.Sum(x => x.FinalAmount),
            ThisMonth = ordersThisMonth.Sum(x => x.FinalAmount),
            ThisYear = ordersThisYear.Sum(x => x.FinalAmount)
        };

        // Orders
        var allOrdersToday = await _unitOfWork.OrderRepository.WhereAsync(x => x.OrderDate.Date == today);
        var allOrdersThisWeek = await _unitOfWork.OrderRepository.WhereAsync(
            x => x.OrderDate >= startOfWeek && x.OrderDate <= today);
        var allOrdersThisMonth = await _unitOfWork.OrderRepository.WhereAsync(
            x => x.OrderDate >= startOfMonth && x.OrderDate <= today);
        var allOrders = await _unitOfWork.OrderRepository.WhereAsync(x => true);
        var successfulOrders = allOrders.Count(x => x.PaymentStatus == PaymentStatusConstant.PAID);
        var totalOrders = allOrders.Count;
        var orderSuccessRate = totalOrders > 0 ? (double)successfulOrders / totalOrders * 100 : 0;

        var orders = new DashboardOrderItem
        {
            Today = allOrdersToday.Count,
            ThisWeek = allOrdersThisWeek.Count,
            ThisMonth = allOrdersThisMonth.Count,
            SuccessRate = orderSuccessRate
        };

        // Bookings
        var bookingsToday = await _unitOfWork.BookingRepository.WhereAsync(
            x => x.BookingDate.Date == today);
        var bookingsThisWeek = await _unitOfWork.BookingRepository.WhereAsync(
            x => x.BookingDate >= startOfWeek && x.BookingDate <= today);
        var bookingsThisMonth = await _unitOfWork.BookingRepository.WhereAsync(
            x => x.BookingDate >= startOfMonth && x.BookingDate <= today);
        var allBookings = await _unitOfWork.BookingRepository.WhereAsync(x => true);
        var pendingBookings = allBookings.Count(x => x.BookingStatus == BookingStatusConstant.PENDING);
        var completedBookings = allBookings.Count(x => x.BookingStatus == BookingStatusConstant.COMPLETED);

        var bookings = new DashboardBookingItem
        {
            Today = bookingsToday.Count,
            ThisWeek = bookingsThisWeek.Count,
            ThisMonth = bookingsThisMonth.Count,
            Pending = pendingBookings,
            Completed = completedBookings
        };

        // Payment
        var allPaidOrders = allOrders.Count(x => x.PaymentStatus == PaymentStatusConstant.PAID);
        var allPendingOrders = allOrders.Count(x => x.PaymentStatus == PaymentStatusConstant.PENDING);
        var paymentSuccessRate = totalOrders > 0 ? (double)allPaidOrders / totalOrders * 100 : 0;

        var payment = new DashboardPaymentItem
        {
            SuccessRate = paymentSuccessRate,
            SuccessfulPayments = allPaidOrders,
            FailedPayments = allPendingOrders
        };

        // Customers
        var customers = await _unitOfWork.CustomerRepository.WhereAsync(x => true);
        var newCustomersToday = customers.Count(x => x.CreatedAt.Date == today);
        var newCustomersThisWeek = customers.Count(x => x.CreatedAt >= startOfWeek && x.CreatedAt <= today);
        var newCustomersThisMonth = customers.Count(x => x.CreatedAt >= startOfMonth && x.CreatedAt <= today);

        var customer = new DashboardCustomerItem
        {
            NewToday = newCustomersToday,
            NewThisWeek = newCustomersThisWeek,
            NewThisMonth = newCustomersThisMonth,
            Total = customers.Count
        };

        // Tasks
        var allTasks = await _unitOfWork.TaskRepository.WhereAsync(x => true);
        var dailyTasks = await _unitOfWork.DailyTaskRepository.WhereAsync(x => true);
        var inProgressTasks = dailyTasks.Count(x => x.Status == DailyTaskStatusConstant.IN_PROGRESS);
        var pendingTasks = dailyTasks.Count(x => x.Status == DailyTaskStatusConstant.SCHEDULED);
        var completedTasks = dailyTasks.Count(x => x.Status == DailyTaskStatusConstant.COMPLETED);
        var totalDailyTasks = dailyTasks.Count;
        var taskCompletionRate = totalDailyTasks > 0 ? (double)completedTasks / totalDailyTasks * 100 : 0;

        var task = new DashboardTaskItem
        {
            InProgress = inProgressTasks,
            Pending = pendingTasks,
            Completed = completedTasks,
            CompletionRate = taskCompletionRate
        };

        // Employees
        var employees = await _unitOfWork.EmployeeRepository.WhereAsync(x => true);
        var activeEmployees = employees.Count; // All non-deleted are active
        var workingToday = employees.Count; // Simplified - would need work schedule check

        var employee = new DashboardEmployeeItem
        {
            Active = activeEmployees,
            WorkingToday = workingToday
        };

        return new DashboardOverviewResponse
        {
            Revenue = revenue,
            Orders = orders,
            Bookings = bookings,
            Payment = payment,
            Customers = customer,
            Tasks = task,
            Employees = employee
        };
    }

    #endregion

    #region Helper Methods

    private static (DateTime Start, DateTime End) GetDateRange(DateRangeFilter filter)
    {
        DateTime startDate;
        DateTime endDate;

        if (filter.StartDate.HasValue && filter.EndDate.HasValue)
        {
            startDate = filter.StartDate.Value.Date;
            endDate = filter.EndDate.Value.Date.AddDays(1).AddSeconds(-1);
        }
        else if (filter.StartDate.HasValue)
        {
            startDate = filter.StartDate.Value.Date;
            endDate = DateTime.UtcNow;
        }
        else if (filter.EndDate.HasValue)
        {
            endDate = filter.EndDate.Value.Date.AddDays(1).AddSeconds(-1);
            startDate = endDate.AddMonths(-1);
        }
        else
        {
            // Default to last 30 days
            endDate = DateTime.UtcNow;
            startDate = endDate.AddDays(-30);
        }

        return (startDate, endDate);
    }

    private static (DateTime Start, DateTime End) GetPreviousPeriod(DateTime? startDate, DateTime? endDate, string? period)
    {
        DateTime start;
        DateTime end;

        if (startDate.HasValue && endDate.HasValue)
        {
            var duration = endDate.Value - startDate.Value;
            start = startDate.Value - duration;
            end = startDate.Value.AddSeconds(-1);
        }
        else
        {
            var now = DateTime.UtcNow;
            switch (period?.ToLower())
            {
                case "day":
                    start = now.AddDays(-2).Date;
                    end = now.AddDays(-1).Date.AddDays(1).AddSeconds(-1);
                    break;
                case "week":
                    var weekStart = now.AddDays(-(int)now.DayOfWeek);
                    start = weekStart.AddDays(-7).Date;
                    end = weekStart.AddSeconds(-1);
                    break;
                case "year":
                    start = new DateTime(now.Year - 1, 1, 1);
                    end = new DateTime(now.Year, 1, 1).AddSeconds(-1);
                    break;
                case "month":
                default:
                    start = new DateTime(now.Year, now.Month, 1).AddMonths(-1);
                    end = new DateTime(now.Year, now.Month, 1).AddSeconds(-1);
                    break;
            }
        }

        return (start, end);
    }

    private IGrouping<string, Order>[] GroupByPeriod(List<Order> orders, string period)
    {
        return period.ToLower() switch
        {
            "day" => orders.GroupBy(x => x.OrderDate.ToString("yyyy-MM-dd")).ToArray(),
            "week" => orders.GroupBy(x => $"{GetWeekOfYear(x.OrderDate)}-{x.OrderDate.Year}").ToArray(),
            "year" => orders.GroupBy(x => x.OrderDate.Year.ToString()).ToArray(),
            _ => orders.GroupBy(x => x.OrderDate.ToString("yyyy-MM")).ToArray() // month
        };
    }

    private IGrouping<string, T>[] GroupByPeriod<T>(List<T> items, string period, Func<T, DateTime> dateSelector)
    {
        return period.ToLower() switch
        {
            "day" => items.GroupBy(x => dateSelector(x).ToString("yyyy-MM-dd")).ToArray(),
            "week" => items.GroupBy(x => $"{GetWeekOfYear(dateSelector(x))}-{dateSelector(x).Year}").ToArray(),
            "year" => items.GroupBy(x => dateSelector(x).Year.ToString()).ToArray(),
            _ => items.GroupBy(x => dateSelector(x).ToString("yyyy-MM")).ToArray() // month
        };
    }

    private static int GetWeekOfYear(DateTime date)
    {
        var culture = System.Globalization.CultureInfo.CurrentCulture;
        var calendar = culture.Calendar;
        return calendar.GetWeekOfYear(date, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
    }

    private static string GetAgeGroup(int age)
    {
        return age switch
        {
            <= 1 => "0-1",
            <= 5 => "2-5",
            <= 10 => "6-10",
            _ => "11+"
        };
    }

    #endregion
}