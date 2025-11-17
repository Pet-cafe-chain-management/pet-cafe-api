using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.LeaveRequestModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Services.Commons;
using PetCafe.Domain.Constants;
using PetCafe.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace PetCafe.Application.Services;

public interface ILeaveRequestService
{
    Task<LeaveRequest> CreateAsync(LeaveRequestCreateModel model);
    Task<LeaveRequest> UpdateAsync(Guid id, LeaveRequestUpdateModel model);
    Task<LeaveRequest> ApproveAsync(Guid id, LeaveRequestReviewModel model);
    Task<LeaveRequest> RejectAsync(Guid id, LeaveRequestReviewModel model);
    Task<bool> CancelAsync(Guid id);
    Task<LeaveRequest> GetByIdAsync(Guid id);
    Task<BasePagingResponseModel<LeaveRequest>> GetAllAsync(LeaveRequestFilterQuery query);
}

public class LeaveRequestService(
    IUnitOfWork _unitOfWork,
    ICurrentTime _currentTime,
    IClaimsService _claimsService,
    ILeaveRequestAssignmentService _assignmentService,
    INotificationService _notificationService
) : ILeaveRequestService
{
    public async Task<LeaveRequest> CreateAsync(LeaveRequestCreateModel model)
    {
        // Validate employee exists
        var employee = await _unitOfWork.EmployeeRepository.GetByIdAsync(model.EmployeeId)
            ?? throw new BadRequestException("Không tìm thấy nhân viên!");

        if (!employee.IsActive)
        {
            throw new BadRequestException("Nhân viên hiện không đang hoạt động!");
        }

        // Validate replacement employee for ADVANCE type
        if (model.LeaveType == LeaveRequestTypeConstant.ADVANCE)
        {
            if (model.ReplacementEmployeeId == null)
            {
                throw new BadRequestException("Nhân viên thay thế là bắt buộc cho nghỉ phép có kế hoạch!");
            }

            var replacementEmployee = await _unitOfWork.EmployeeRepository.GetByIdAsync(model.ReplacementEmployeeId.Value)
                ?? throw new BadRequestException("Không tìm thấy nhân viên thay thế!");

            if (!replacementEmployee.IsActive)
            {
                throw new BadRequestException("Nhân viên thay thế hiện không đang hoạt động!");
            }

            if (replacementEmployee.Id == employee.Id)
            {
                throw new BadRequestException("Nhân viên thay thế không thể là chính nhân viên nghỉ phép!");
            }
        }

        // Check if employee already has a pending leave request for the same date
        var existingRequest = await _unitOfWork.LeaveRequestRepository.FirstOrDefaultAsync(
            lr => lr.EmployeeId == model.EmployeeId
                && lr.LeaveDate.Date == model.LeaveDate.Date
                && lr.Status == LeaveRequestStatusConstant.PENDING
        );

        if (existingRequest != null)
        {
            throw new BadRequestException($"Đã có đơn nghỉ phép đang chờ duyệt cho ngày {model.LeaveDate:dd/MM/yyyy}!");
        }

        var leaveRequest = _unitOfWork.Mapper.Map<LeaveRequest>(model);
        leaveRequest.Status = LeaveRequestStatusConstant.PENDING;

        await _unitOfWork.LeaveRequestRepository.AddAsync(leaveRequest);
        await _unitOfWork.SaveChangesAsync();

        // Load employee with account for notification
        employee = await _unitOfWork.EmployeeRepository.GetByIdAsync(
            model.EmployeeId,
            includeFunc: q => q.Include(e => e.Account)
        ) ?? throw new BadRequestException("Không tìm thấy nhân viên!");

        // Send notification to managers and team leaders
        var employeeTeams = await _unitOfWork.TeamMemberRepository.WhereAsync(
            tm => tm.EmployeeId == model.EmployeeId && !tm.IsDeleted,
            includeFunc: q => q.Include(tm => tm.Team).ThenInclude(t => t.Leader).ThenInclude(l => l.Account)
        );

        var managerAccounts = await _unitOfWork.AccountRepository.WhereAsync(
            a => a.Role == RoleConstants.MANAGER && a.IsActive && !a.IsDeleted
        );

        var notificationTasks = new List<Task>();

        // Notify managers
        foreach (var managerAccount in managerAccounts)
        {
            notificationTasks.Add(_notificationService.SendNotificationAsync(
                managerAccount.Id,
                "Đơn nghỉ phép mới",
                $"Nhân viên {employee.FullName} đã nộp đơn nghỉ phép cho ngày {model.LeaveDate:dd/MM/yyyy}",
                "LeaveRequest",
                "Normal",
                leaveRequest.Id,
                "LeaveRequest"
            ));
        }

        // Notify team leaders
        foreach (var teamMember in employeeTeams)
        {
            if (teamMember.Team?.Leader?.AccountId != null)
            {
                notificationTasks.Add(_notificationService.SendNotificationAsync(
                    teamMember.Team.Leader.AccountId,
                    "Đơn nghỉ phép mới",
                    $"Nhân viên {employee.FullName} trong team {teamMember.Team.Name} đã nộp đơn nghỉ phép cho ngày {model.LeaveDate:dd/MM/yyyy}",
                    "LeaveRequest",
                    "Normal",
                    leaveRequest.Id,
                    "LeaveRequest"
                ));
            }
        }

        await Task.WhenAll(notificationTasks);

        return leaveRequest;
    }

    public async Task<LeaveRequest> UpdateAsync(Guid id, LeaveRequestUpdateModel model)
    {
        var leaveRequest = await _unitOfWork.LeaveRequestRepository.GetByIdAsync(
            id,
            includeFunc: x => x
                .Include(lr => lr.Employee)
                .Include(lr => lr.ReplacementEmployee!)
        ) ?? throw new NotFoundException("Không tìm thấy đơn nghỉ phép!");

        // Only allow update if status is PENDING
        if (leaveRequest.Status != LeaveRequestStatusConstant.PENDING)
        {
            throw new BadRequestException("Chỉ có thể cập nhật đơn nghỉ phép đang chờ duyệt!");
        }

        // Validate replacement employee for ADVANCE type
        if (model.LeaveType == LeaveRequestTypeConstant.ADVANCE && model.ReplacementEmployeeId.HasValue)
        {
            var replacementEmployee = await _unitOfWork.EmployeeRepository.GetByIdAsync(model.ReplacementEmployeeId.Value)
                ?? throw new BadRequestException("Không tìm thấy nhân viên thay thế!");

            if (!replacementEmployee.IsActive)
            {
                throw new BadRequestException("Nhân viên thay thế hiện không đang hoạt động!");
            }
        }

        _unitOfWork.Mapper.Map(model, leaveRequest);
        _unitOfWork.LeaveRequestRepository.Update(leaveRequest);
        await _unitOfWork.SaveChangesAsync();

        return leaveRequest;
    }

    public async Task<LeaveRequest> ApproveAsync(Guid id, LeaveRequestReviewModel model)
    {
        var leaveRequest = await _unitOfWork.LeaveRequestRepository.GetByIdAsync(
            id,
            includeFunc: x => x
                .Include(lr => lr.Employee)
                .Include(lr => lr.ReplacementEmployee!)
        ) ?? throw new NotFoundException("Không tìm thấy đơn nghỉ phép!");

        if (leaveRequest.Status != LeaveRequestStatusConstant.PENDING)
        {
            throw new BadRequestException("Chỉ có thể duyệt đơn nghỉ phép đang chờ duyệt!");
        }

        // Check if user has permission (Manager or Leader of employee's team)
        var currentUserId = _claimsService.GetCurrentUser;
        var currentUserRole = _claimsService.GetCurrentUserRole;

        if (currentUserRole != RoleConstants.MANAGER)
        {
            // Check if current user is leader of any team that contains the employee
            var employeeTeams = await _unitOfWork.TeamMemberRepository.WhereAsync(
                tm => tm.EmployeeId == leaveRequest.EmployeeId && !tm.IsDeleted,
                includeFunc: q => q.Include(tm => tm.Team)
            );

            var isLeader = employeeTeams.Any(tm => tm.Team.LeaderId == currentUserId);
            if (!isLeader)
            {
                throw new BadRequestException("Bạn không có quyền duyệt đơn nghỉ phép này!");
            }
        }

        leaveRequest.Status = LeaveRequestStatusConstant.APPROVED;
        leaveRequest.ReviewedBy = currentUserId;
        leaveRequest.ReviewedAt = _currentTime.GetCurrentTime;
        leaveRequest.ReviewNotes = model.ReviewNotes;

        _unitOfWork.LeaveRequestRepository.Update(leaveRequest);
        await _unitOfWork.SaveChangesAsync();

        // Process assignment based on leave type
        if (leaveRequest.LeaveType == LeaveRequestTypeConstant.ADVANCE)
        {
            await _assignmentService.ProcessAdvanceLeaveAsync(leaveRequest);
        }
        else if (leaveRequest.LeaveType == LeaveRequestTypeConstant.EMERGENCY)
        {
            await _assignmentService.ProcessEmergencyLeaveAsync(leaveRequest);
        }

        // Send notification to employee
        var employee = await _unitOfWork.EmployeeRepository.GetByIdAsync(
            leaveRequest.EmployeeId,
            includeFunc: q => q.Include(e => e.Account)
        );

        if (employee?.AccountId != null)
        {
            await _notificationService.SendNotificationAsync(
                employee.AccountId,
                "Đơn nghỉ phép đã được duyệt",
                $"Đơn nghỉ phép của bạn cho ngày {leaveRequest.LeaveDate:dd/MM/yyyy} đã được duyệt.",
                "LeaveRequest",
                "Normal",
                leaveRequest.Id,
                "LeaveRequest"
            );
        }

        return leaveRequest;
    }

    public async Task<LeaveRequest> RejectAsync(Guid id, LeaveRequestReviewModel model)
    {
        var leaveRequest = await _unitOfWork.LeaveRequestRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Không tìm thấy đơn nghỉ phép!");

        if (leaveRequest.Status != LeaveRequestStatusConstant.PENDING)
        {
            throw new BadRequestException("Chỉ có thể từ chối đơn nghỉ phép đang chờ duyệt!");
        }

        // Check permission (same as approve)
        var currentUserId = _claimsService.GetCurrentUser;
        var currentUserRole = _claimsService.GetCurrentUserRole;

        if (currentUserRole != RoleConstants.MANAGER)
        {
            var employeeTeams = await _unitOfWork.TeamMemberRepository.WhereAsync(
                tm => tm.EmployeeId == leaveRequest.EmployeeId && !tm.IsDeleted,
                includeFunc: q => q.Include(tm => tm.Team)
            );

            var isLeader = employeeTeams.Any(tm => tm.Team.LeaderId == currentUserId);
            if (!isLeader)
            {
                throw new BadRequestException("Bạn không có quyền từ chối đơn nghỉ phép này!");
            }
        }

        leaveRequest.Status = LeaveRequestStatusConstant.REJECTED;
        leaveRequest.ReviewedBy = currentUserId;
        leaveRequest.ReviewedAt = _currentTime.GetCurrentTime;
        leaveRequest.ReviewNotes = model.ReviewNotes;

        _unitOfWork.LeaveRequestRepository.Update(leaveRequest);
        await _unitOfWork.SaveChangesAsync();

        // Send notification to employee
        var employee = await _unitOfWork.EmployeeRepository.GetByIdAsync(
            leaveRequest.EmployeeId,
            includeFunc: q => q.Include(e => e.Account)
        );

        if (employee?.AccountId != null)
        {
            await _notificationService.SendNotificationAsync(
                employee.AccountId,
                "Đơn nghỉ phép đã bị từ chối",
                $"Đơn nghỉ phép của bạn cho ngày {leaveRequest.LeaveDate:dd/MM/yyyy} đã bị từ chối.{(string.IsNullOrEmpty(model.ReviewNotes) ? "" : $" Lý do: {model.ReviewNotes}")}",
                "LeaveRequest",
                "Normal",
                leaveRequest.Id,
                "LeaveRequest"
            );
        }

        return leaveRequest;
    }

    public async Task<bool> CancelAsync(Guid id)
    {
        var leaveRequest = await _unitOfWork.LeaveRequestRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Không tìm thấy đơn nghỉ phép!");

        if (leaveRequest.Status != LeaveRequestStatusConstant.PENDING)
        {
            throw new BadRequestException("Chỉ có thể hủy đơn nghỉ phép đang chờ duyệt!");
        }

        // Check if current user is the employee who created the request
        var currentUserId = _claimsService.GetCurrentUser;
        if (leaveRequest.EmployeeId != currentUserId)
        {
            throw new BadRequestException("Bạn chỉ có thể hủy đơn nghỉ phép của chính mình!");
        }

        leaveRequest.Status = LeaveRequestStatusConstant.CANCELLED;
        _unitOfWork.LeaveRequestRepository.Update(leaveRequest);
        return await _unitOfWork.SaveChangesAsync();
    }

    public async Task<LeaveRequest> GetByIdAsync(Guid id)
    {
        return await _unitOfWork.LeaveRequestRepository.GetByIdAsync(
            id,
            includeFunc: x => x
                .Include(lr => lr.Employee)
                .Include(lr => lr.ReplacementEmployee!)
                .Include(lr => lr.Reviewer!)
        ) ?? throw new NotFoundException("Không tìm thấy đơn nghỉ phép!");
    }

    public async Task<BasePagingResponseModel<LeaveRequest>> GetAllAsync(LeaveRequestFilterQuery query)
    {
        Expression<Func<LeaveRequest, bool>> filter = x => true;

        if (query.EmployeeId.HasValue)
        {
            filter = filter.Combine(x => x.EmployeeId == query.EmployeeId.Value);
        }

        if (query.ReviewedBy.HasValue)
        {
            filter = filter.Combine(x => x.ReviewedBy == query.ReviewedBy.Value);
        }

        if (!string.IsNullOrEmpty(query.Status))
        {
            filter = filter.Combine(x => x.Status == query.Status);
        }

        if (!string.IsNullOrEmpty(query.LeaveType))
        {
            filter = filter.Combine(x => x.LeaveType == query.LeaveType);
        }

        if (query.FromDate.HasValue)
        {
            filter = filter.Combine(x => x.LeaveDate >= query.FromDate.Value);
        }

        if (query.ToDate.HasValue)
        {
            filter = filter.Combine(x => x.LeaveDate <= query.ToDate.Value);
        }

        var (Pagination, Entities) = await _unitOfWork.LeaveRequestRepository.ToPagination(
            pageIndex: query.Page ?? 0,
            pageSize: query.Limit ?? 10,
            filter: filter,
            searchTerm: query.Q,
            searchFields: ["Reason", "ReviewNotes"],
            sortOrders: query.OrderBy?.ToDictionary(
                k => k.OrderColumn ?? "CreatedAt",
                v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
            ) ?? new Dictionary<string, bool> { { "CreatedAt", false } },
            includeFunc: x => x
                .Include(lr => lr.Employee)
                .Include(lr => lr.ReplacementEmployee!)
                .Include(lr => lr.Reviewer!)
        );

        return BasePagingResponseModel<LeaveRequest>.CreateInstance(Entities, Pagination);
    }
}

// Extension method to combine expressions
public static class ExpressionExtensions
{
    public static Expression<Func<T, bool>> Combine<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    {
        var parameter = Expression.Parameter(typeof(T));
        var body = Expression.AndAlso(
            Expression.Invoke(first, parameter),
            Expression.Invoke(second, parameter)
        );
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
}

