using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.StatisticsModels;
using PetCafe.Application.Services;
using PetCafe.Domain.Constants;

namespace PetCafe.WebApi.Controllers;

public class StatisticController(
    IStatisticsService _statisticsService
) : BaseController
{
    #region 1. DOANH THU VÀ BÁN HÀNG

    [HttpGet("revenue")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> GetRevenueStatistics([FromQuery] TimePeriodFilter filter)
    {
        var result = await _statisticsService.GetRevenueStatisticsAsync(filter);
        return Ok(result);
    }

    [HttpGet("orders")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> GetOrderStatistics([FromQuery] TimePeriodFilter filter)
    {
        var result = await _statisticsService.GetOrderStatisticsAsync(filter);
        return Ok(result);
    }

    [HttpGet("products")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> GetProductStatistics([FromQuery] TimePeriodFilter filter)
    {
        var result = await _statisticsService.GetProductStatisticsAsync(filter);
        return Ok(result);
    }

    #endregion

    #region 2. THỐNG KÊ DỊCH VỤ

    [HttpGet("services")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> GetServiceStatistics([FromQuery] TimePeriodFilter filter)
    {
        var result = await _statisticsService.GetServiceStatisticsAsync(filter);
        return Ok(result);
    }

    [HttpGet("slots")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> GetSlotStatistics([FromQuery] DateRangeFilter filter)
    {
        var result = await _statisticsService.GetSlotStatisticsAsync(filter);
        return Ok(result);
    }

    [HttpGet("feedbacks")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> GetFeedbackStatistics()
    {
        var result = await _statisticsService.GetFeedbackStatisticsAsync();
        return Ok(result);
    }

    #endregion

    #region 3. THỐNG KÊ THÚ CƯNG

    [HttpGet("pets")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> GetPetStatistics()
    {
        var result = await _statisticsService.GetPetStatisticsAsync();
        return Ok(result);
    }

    [HttpGet("pets/health")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> GetPetHealthStatistics()
    {
        var result = await _statisticsService.GetPetHealthStatisticsAsync();
        return Ok(result);
    }

    [HttpGet("pets/groups")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> GetPetGroupStatistics()
    {
        var result = await _statisticsService.GetPetGroupStatisticsAsync();
        return Ok(result);
    }

    #endregion

    #region 4. THỐNG KÊ NHÂN SỰ

    [HttpGet("employees")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> GetEmployeeStatistics()
    {
        var result = await _statisticsService.GetEmployeeStatisticsAsync();
        return Ok(result);
    }

    [HttpGet("teams")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> GetTeamStatistics()
    {
        var result = await _statisticsService.GetTeamStatisticsAsync();
        return Ok(result);
    }

    [HttpGet("employees/performance")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> GetEmployeePerformance([FromQuery] TimePeriodFilter filter)
    {
        var result = await _statisticsService.GetEmployeePerformanceAsync(filter);
        return Ok(result);
    }

    #endregion

    #region 5. THỐNG KÊ TASK & CÔNG VIỆC

    [HttpGet("tasks")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> GetTaskStatistics()
    {
        var result = await _statisticsService.GetTaskStatisticsAsync();
        return Ok(result);
    }

    [HttpGet("tasks/daily")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> GetDailyTaskStatistics([FromQuery] TimePeriodFilter filter)
    {
        var result = await _statisticsService.GetDailyTaskStatisticsAsync(filter);
        return Ok(result);
    }

    [HttpGet("work-shifts")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> GetWorkShiftStatistics()
    {
        var result = await _statisticsService.GetWorkShiftStatisticsAsync();
        return Ok(result);
    }

    #endregion

    #region 6. THỐNG KÊ KHÁCH HÀNG

    [HttpGet("customers")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> GetCustomerStatistics([FromQuery] TimePeriodFilter filter)
    {
        var result = await _statisticsService.GetCustomerStatisticsAsync(filter);
        return Ok(result);
    }

    #endregion

    #region 7. THỐNG KÊ KHO & VẬT TƯ

    [HttpGet("inventory")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> GetInventoryStatistics()
    {
        var result = await _statisticsService.GetInventoryStatisticsAsync();
        return Ok(result);
    }

    #endregion

    #region 8. THỐNG KÊ TỔNG QUAN (Dashboard)

    [HttpGet("dashboard/overview")]
    [Authorize(Roles = RoleConstants.MANAGER)]
    public async Task<IActionResult> GetDashboardOverview()
    {
        var result = await _statisticsService.GetDashboardOverviewAsync();
        return Ok(result);
    }

    #endregion
}

