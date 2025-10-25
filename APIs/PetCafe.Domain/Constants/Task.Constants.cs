namespace PetCafe.Domain.Constants;

public static class TaskStatusConstant
{
    public const string ACTIVE = "ACTIVE";
    public const string INACTIVE = "INACTIVE";
}
public static class TaskPriorityConstant
{
    public const string LOW = "LOW";
    public const string MEDIUM = "MEDIUM";
    public const string HIGH = "HIGH";
    public const string URGENT = "URGENT";
}

public static class TaskPriorityConstants
{
    public static readonly List<string> ALL_PRIORITIES = [TaskPriorityConstant.LOW, TaskPriorityConstant.MEDIUM, TaskPriorityConstant.HIGH, TaskPriorityConstant.URGENT];
}

public static class TaskStatusConstants
{
    public static readonly List<string> ALL_STATUSES = [TaskStatusConstant.ACTIVE, TaskStatusConstant.INACTIVE];
}