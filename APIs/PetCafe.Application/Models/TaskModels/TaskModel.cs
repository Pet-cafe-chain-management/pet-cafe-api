using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Domain.Constants;
using FluentValidation;
namespace PetCafe.Application.Models.TaskModels;

public class TaskCreateModel
{
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string Priority { get; set; } = TaskPriorityConstant.MEDIUM; // Low, Medium, High, Urgent
    public string Status { get; set; } = TaskStatusConstant.ACTIVE; // Pending, InProgress, Completed, Cancelled
    public int? EstimatedHours { get; set; }
    public bool IsRecurring { get; set; } = false;
    public bool IsPublic { get; set; } = false;

    public Guid WorkTypeId { get; set; }
}

public class TaskUpdateModel : TaskCreateModel
{
}

public class TaskCreateModelValidator : AbstractValidator<TaskCreateModel>
{
    public TaskCreateModelValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Tên task không được để trống");
        RuleFor(x => x.WorkTypeId)
            .NotEmpty().WithMessage("ID WorkType không được để trống");
        RuleFor(x => x.Priority)
            .NotEmpty().WithMessage("Priority không được để trống")
            .Must(x => TaskPriorityConstants.ALL_PRIORITIES.Contains(x)).WithMessage("Priority không hợp lệ");
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status không được để trống")
            .Must(x => TaskStatusConstants.ALL_STATUSES.Contains(x)).WithMessage("Status không hợp lệ");
        RuleFor(x => x.EstimatedHours)
            .GreaterThan(0).WithMessage("EstimatedHours phải lớn hơn 0");
        RuleFor(x => x.IsRecurring)
            .NotNull().WithMessage("IsRecurring không được để trống");

    }
}

public class TaskUpdateModelValidator : AbstractValidator<TaskUpdateModel>
{
    public TaskUpdateModelValidator()
    {
        Include(new TaskCreateModelValidator());
    }
}

public class TaskFilterQuery : FilterQuery
{
    [FromQuery(Name = "work_type_id")]
    public Guid? WorkTypeId { get; set; }

    [FromQuery(Name = "status")]
    public string? Status { get; set; }

    [FromQuery(Name = "priority")]
    public string? Priority { get; set; }

    [FromQuery(Name = "is_public")]
    public bool? IsPublic { get; set; }
    [FromQuery(Name = "is_recurring")]
    public bool? IsRecurring { get; set; }
}