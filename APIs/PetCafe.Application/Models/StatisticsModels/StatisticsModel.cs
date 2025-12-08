using Microsoft.AspNetCore.Mvc;

namespace PetCafe.Application.Models.StatisticsModels;

// Common Models
public class DateRangeFilter
{
    [FromQuery(Name = "start_date")]
    public DateTime? StartDate { get; set; }

    [FromQuery(Name = "end_date")]
    public DateTime? EndDate { get; set; }
}

public class TimePeriodFilter : DateRangeFilter
{
    [FromQuery(Name = "period")]
    public string? Period { get; set; } // "day", "week", "month", "year"
}

// 1. DOANH THU VÀ BÁN HÀNG
public class RevenueStatisticsResponse
{
    public double TotalRevenue { get; set; }
    public double TotalRevenueByPaymentMethod { get; set; }
    public double PreviousPeriodRevenue { get; set; }
    public double GrowthRate { get; set; }
    public double AverageOrderValue { get; set; }
    public List<RevenueByPeriodItem> RevenueByPeriod { get; set; } = [];
    public List<RevenueByPaymentMethodItem> RevenueByPaymentMethod { get; set; } = [];
    public List<RevenueByOrderStatusItem> RevenueByOrderStatus { get; set; } = [];
    public List<RevenueByOrderTypeItem> RevenueByOrderType { get; set; } = [];
}

public class RevenueByPeriodItem
{
    public string Period { get; set; } = default!;
    public double Revenue { get; set; }
}

public class RevenueByPaymentMethodItem
{
    public string PaymentMethod { get; set; } = default!;
    public double Revenue { get; set; }
}

public class RevenueByOrderStatusItem
{
    public string Status { get; set; } = default!;
    public double Revenue { get; set; }
}

public class RevenueByOrderTypeItem
{
    public string OrderType { get; set; } = default!;
    public double Revenue { get; set; }
}

public class OrderStatisticsResponse
{
    public int TotalOrders { get; set; }
    public int PreviousPeriodOrders { get; set; }
    public double GrowthRate { get; set; }
    public double AverageOrderProcessingTime { get; set; }
    public List<OrderByStatusItem> OrdersByStatus { get; set; } = [];
    public List<OrderByPeriodItem> OrdersByPeriod { get; set; } = [];
    public List<TopCustomerByOrderItem> TopCustomersByOrderCount { get; set; } = [];
    public List<TopCustomerByRevenueItem> TopCustomersByRevenue { get; set; } = [];
}

public class OrderByStatusItem
{
    public string Status { get; set; } = default!;
    public int Count { get; set; }
}

public class OrderByPeriodItem
{
    public string Period { get; set; } = default!;
    public int Count { get; set; }
}

public class TopCustomerByOrderItem
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = default!;
    public int OrderCount { get; set; }
}

public class TopCustomerByRevenueItem
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = default!;
    public double TotalRevenue { get; set; }
}

public class ProductStatisticsResponse
{
    public List<TopSellingProductItem> TopSellingByQuantity { get; set; } = [];
    public List<TopSellingProductItem> TopSellingByRevenue { get; set; } = [];
    public List<LowStockProductItem> LowStockProducts { get; set; } = [];
    public List<NoSalesProductItem> NoSalesProducts { get; set; } = [];
    public double TotalInventoryValue { get; set; }
}

public class TopSellingProductItem
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public int Quantity { get; set; }
    public double Revenue { get; set; }
}

public class LowStockProductItem
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public int StockQuantity { get; set; }
    public int MinStockLevel { get; set; }
}

public class NoSalesProductItem
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public int StockQuantity { get; set; }
}

// 2. THỐNG KÊ DỊCH VỤ
public class ServiceStatisticsResponse
{
    public int TotalBookings { get; set; }
    public List<BookingByStatusItem> BookingsByStatus { get; set; } = [];
    public double CompletionRate { get; set; }
    public double CancellationRate { get; set; }
    public List<TopServiceItem> TopServices { get; set; } = [];
    public List<ServiceRevenueItem> ServiceRevenues { get; set; } = [];
    public List<BookingByPeriodItem> BookingsByPeriod { get; set; } = [];
}

public class BookingByStatusItem
{
    public string Status { get; set; } = default!;
    public int Count { get; set; }
}

public class BookingByPeriodItem
{
    public string Period { get; set; } = default!;
    public int Count { get; set; }
}

public class TopServiceItem
{
    public Guid ServiceId { get; set; }
    public string ServiceName { get; set; } = default!;
    public int BookingCount { get; set; }
}

public class ServiceRevenueItem
{
    public Guid ServiceId { get; set; }
    public string ServiceName { get; set; } = default!;
    public double Revenue { get; set; }
}

public class SlotStatisticsResponse
{
    public List<SlotAvailabilityItem> SlotAvailabilityByDay { get; set; } = [];
    public double UtilizationRate { get; set; }
    public List<SlotByTimeSlotItem> SlotByTimeSlot { get; set; } = [];
    public List<SlotByAreaItem> SlotByArea { get; set; } = [];
}

public class SlotAvailabilityItem
{
    public DateTime Date { get; set; }
    public int Available { get; set; }
    public int Occupied { get; set; }
    public int Total { get; set; }
}

public class SlotByTimeSlotItem
{
    public string TimeSlot { get; set; } = default!;
    public int Count { get; set; }
}

public class SlotByAreaItem
{
    public Guid AreaId { get; set; }
    public string AreaName { get; set; } = default!;
    public int SlotCount { get; set; }
}

public class FeedbackStatisticsResponse
{
    public int TotalFeedbacks { get; set; }
    public double AverageRating { get; set; }
    public List<RatingDistributionItem> RatingDistribution { get; set; } = [];
    public List<TopRatedServiceItem> TopRatedServices { get; set; } = [];
}

public class RatingDistributionItem
{
    public int Rating { get; set; }
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class TopRatedServiceItem
{
    public Guid ServiceId { get; set; }
    public string ServiceName { get; set; } = default!;
    public double AverageRating { get; set; }
    public int FeedbackCount { get; set; }
}

// 3. THỐNG KÊ THÚ CƯNG
public class PetStatisticsResponse
{
    public int TotalPets { get; set; }
    public List<PetBySpeciesItem> PetsBySpecies { get; set; } = [];
    public List<PetByBreedItem> PetsByBreed { get; set; } = [];
    public List<PetByAgeGroupItem> PetsByAgeGroup { get; set; } = [];
    public List<PetByGenderItem> PetsByGender { get; set; } = [];
    public List<PetArrivalByMonthItem> PetArrivalsByMonth { get; set; } = [];
}

public class PetBySpeciesItem
{
    public Guid SpeciesId { get; set; }
    public string SpeciesName { get; set; } = default!;
    public int Count { get; set; }
}

public class PetByBreedItem
{
    public Guid BreedId { get; set; }
    public string BreedName { get; set; } = default!;
    public int Count { get; set; }
}

public class PetByAgeGroupItem
{
    public string AgeGroup { get; set; } = default!; // "0-1", "2-5", "6-10", "11+"
    public int Count { get; set; }
}

public class PetByGenderItem
{
    public string Gender { get; set; } = default!;
    public int Count { get; set; }
}

public class PetArrivalByMonthItem
{
    public string Month { get; set; } = default!;
    public int Count { get; set; }
}

public class PetHealthStatisticsResponse
{
    public int PetsWithHealthRecords { get; set; }
    public VaccinationStatusItem VaccinationStatus { get; set; } = new();
    public List<HealthCheckByMonthItem> HealthChecksByMonth { get; set; } = [];
}


public class VaccinationStatusItem
{
    public int VaccinatedCount { get; set; }
    public int NotVaccinatedCount { get; set; }
    public double VaccinationRate { get; set; }
}

public class HealthCheckByMonthItem
{
    public string Month { get; set; } = default!;
    public int Count { get; set; }
}

public class PetGroupStatisticsResponse
{
    public int TotalGroups { get; set; }
    public List<PetGroupDetailItem> PetGroupDetails { get; set; } = [];
}

public class PetGroupDetailItem
{
    public Guid GroupId { get; set; }
    public string GroupName { get; set; } = default!;
    public int PetCount { get; set; }
}

// 4. THỐNG KÊ NHÂN SỰ
public class EmployeeStatisticsResponse
{
    public int TotalEmployees { get; set; }
    public int ActiveEmployees { get; set; }
    public int InactiveEmployees { get; set; }
    public List<EmployeeBySubRoleItem> EmployeesBySubRole { get; set; } = [];
    public List<NewEmployeeByMonthItem> NewEmployeesByMonth { get; set; } = [];
    public double TotalSalaryCost { get; set; }
}

public class EmployeeBySubRoleItem
{
    public string SubRole { get; set; } = default!;
    public int Count { get; set; }
}

public class NewEmployeeByMonthItem
{
    public string Month { get; set; } = default!;
    public int Count { get; set; }
}

public class TeamStatisticsResponse
{
    public int TotalTeams { get; set; }
    public int ActiveTeams { get; set; }
    public int InactiveTeams { get; set; }
    public List<TeamByStatusItem> TeamsByStatus { get; set; } = [];
    public double AverageMembersPerTeam { get; set; }
}

public class TeamByStatusItem
{
    public string Status { get; set; } = default!;
    public int Count { get; set; }
}

public class EmployeePerformanceResponse
{
    public List<TopPerformingEmployeeItem> TopPerformingEmployees { get; set; } = [];
    public List<EmployeeBookingCompletionItem> EmployeeBookingCompletions { get; set; } = [];
    public List<EmployeeTaskCompletionItem> EmployeeTaskCompletions { get; set; } = [];
}

public class TopPerformingEmployeeItem
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = default!;
    public int BookingCount { get; set; }
}

public class EmployeeBookingCompletionItem
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = default!;
    public int CompletedBookings { get; set; }
}

public class EmployeeTaskCompletionItem
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = default!;
    public double AverageCompletionTime { get; set; }
}

// 5. THỐNG KÊ TASK & CÔNG VIỆC
public class TaskStatisticsResponse
{
    public int TotalTasks { get; set; }
    public List<TaskByStatusItem> TasksByStatus { get; set; } = [];
    public List<TaskByPriorityItem> TasksByPriority { get; set; } = [];
    public double CompletionRate { get; set; }
    public List<TaskByWorkTypeItem> TasksByWorkType { get; set; } = [];
    public TaskPublicPrivateItem TaskPublicPrivate { get; set; } = new();
}

public class TaskByStatusItem
{
    public string Status { get; set; } = default!;
    public int Count { get; set; }
}

public class TaskByPriorityItem
{
    public string Priority { get; set; } = default!;
    public int Count { get; set; }
}

public class TaskByWorkTypeItem
{
    public Guid WorkTypeId { get; set; }
    public string WorkTypeName { get; set; } = default!;
    public int Count { get; set; }
}

public class TaskPublicPrivateItem
{
    public int PublicTasks { get; set; }
    public int PrivateTasks { get; set; }
}

public class DailyTaskStatisticsResponse
{
    public List<DailyTaskByPeriodItem> DailyTasksByPeriod { get; set; } = [];
    public List<DailyTaskByStatusItem> DailyTasksByStatus { get; set; } = [];
    public List<DailyTaskByTeamItem> DailyTasksByTeam { get; set; } = [];
    public List<OverdueTaskItem> OverdueTasks { get; set; } = [];
}

public class DailyTaskByPeriodItem
{
    public string Period { get; set; } = default!;
    public int Completed { get; set; }
    public int Pending { get; set; }
}

public class DailyTaskByStatusItem
{
    public string Status { get; set; } = default!;
    public int Count { get; set; }
}

public class DailyTaskByTeamItem
{
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = default!;
    public int TaskCount { get; set; }
}

public class OverdueTaskItem
{
    public Guid TaskId { get; set; }
    public string TaskTitle { get; set; } = default!;
    public DateTime AssignedDate { get; set; }
    public DateTime? DueDate { get; set; }
    public int DaysOverdue { get; set; }
}

public class WorkShiftStatisticsResponse
{
    public int TotalWorkShifts { get; set; }
    public List<WorkShiftAssignmentItem> WorkShiftAssignments { get; set; } = [];
    public double UtilizationRate { get; set; }
}

public class WorkShiftAssignmentItem
{
    public Guid WorkShiftId { get; set; }
    public string WorkShiftName { get; set; } = default!;
    public int EmployeeCount { get; set; }
}

// 6. THỐNG KÊ KHÁCH HÀNG
public class CustomerStatisticsResponse
{
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public int InactiveCustomers { get; set; }
    public List<NewCustomerByPeriodItem> NewCustomersByPeriod { get; set; } = [];
    public List<TopCustomerItem> TopCustomersByOrderCount { get; set; } = [];
    public List<TopCustomerItem> TopCustomersByRevenue { get; set; } = [];
    public double TotalLoyaltyPoints { get; set; }
}

public class NewCustomerByPeriodItem
{
    public string Period { get; set; } = default!;
    public int Count { get; set; }
}

public class TopCustomerItem
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = default!;
    public int OrderCount { get; set; }
    public double TotalRevenue { get; set; }
    public int LoyaltyPoints { get; set; }
}

// 7. THỐNG KÊ KHO & VẬT TƯ
public class InventoryStatisticsResponse
{
    public int TotalProducts { get; set; }
    public double TotalInventoryValue { get; set; }
    public List<LowStockItem> LowStockProducts { get; set; } = [];
    public List<LongStockItem> LongStockProducts { get; set; } = [];
}

public class LowStockItem
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public int StockQuantity { get; set; }
    public int MinStockLevel { get; set; }
    public int Difference { get; set; }
}

public class LongStockItem
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public int StockQuantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public int DaysInStock { get; set; }
}

// 8. THỐNG KÊ TỔNG QUAN (Dashboard)
public class DashboardOverviewResponse
{
    public DashboardRevenueItem Revenue { get; set; } = new();
    public DashboardOrderItem Orders { get; set; } = new();
    public DashboardBookingItem Bookings { get; set; } = new();
    public DashboardPaymentItem Payment { get; set; } = new();
    public DashboardCustomerItem Customers { get; set; } = new();
    public DashboardTaskItem Tasks { get; set; } = new();
    public DashboardEmployeeItem Employees { get; set; } = new();
}

public class DashboardRevenueItem
{
    public double Today { get; set; }
    public double ThisWeek { get; set; }
    public double ThisMonth { get; set; }
    public double ThisYear { get; set; }
}

public class DashboardOrderItem
{
    public int Today { get; set; }
    public int ThisWeek { get; set; }
    public int ThisMonth { get; set; }
    public double SuccessRate { get; set; }
}

public class DashboardBookingItem
{
    public int Today { get; set; }
    public int ThisWeek { get; set; }
    public int ThisMonth { get; set; }
    public int Pending { get; set; }
    public int Completed { get; set; }
}

public class DashboardPaymentItem
{
    public double SuccessRate { get; set; }
    public int SuccessfulPayments { get; set; }
    public int FailedPayments { get; set; }
}

public class DashboardCustomerItem
{
    public int NewToday { get; set; }
    public int NewThisWeek { get; set; }
    public int NewThisMonth { get; set; }
    public int Total { get; set; }
}

public class DashboardTaskItem
{
    public int InProgress { get; set; }
    public int Pending { get; set; }
    public int Completed { get; set; }
    public double CompletionRate { get; set; }
}

public class DashboardEmployeeItem
{
    public int Active { get; set; }
    public int WorkingToday { get; set; }
}

