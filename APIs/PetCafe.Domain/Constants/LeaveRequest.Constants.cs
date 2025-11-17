namespace PetCafe.Domain.Constants;

public static class LeaveRequestTypeConstant
{
    public const string ADVANCE = "ADVANCE";
    public const string EMERGENCY = "EMERGENCY";

    public static readonly List<string> ALL_TYPES = [ADVANCE, EMERGENCY];
}

public static class LeaveRequestStatusConstant
{
    public const string PENDING = "PENDING";
    public const string APPROVED = "APPROVED";
    public const string REJECTED = "REJECTED";
    public const string CANCELLED = "CANCELLED";

    public static readonly List<string> ALL_STATUSES = [PENDING, APPROVED, REJECTED, CANCELLED];
}

