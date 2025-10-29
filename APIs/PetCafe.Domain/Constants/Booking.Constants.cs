namespace PetCafe.Domain.Constants;

public static class BookingStatusConstant
{
    public const string PENDING = "PENDING";
    public const string CONFIRMED = "CONFIRMED";
    public const string IN_PROGRESS = "IN_PROGRESS";
    public const string COMPLETED = "COMPLETED";
    public const string CANCELLED = "CANCELLED";
    public static readonly List<string> ALL_STATUSES = [PENDING, CONFIRMED, IN_PROGRESS, COMPLETED, CANCELLED];
}