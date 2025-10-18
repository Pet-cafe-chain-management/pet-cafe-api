namespace PetCafe.Domain.Constants;

public static class DayConstant
{

    public const string MONDAY = "MONDAY";

    public const string TUESDAY = "TUESDAY";

    public const string WEDNESDAY = "WEDNESDAY";

    public const string THURSDAY = "THURSDAY";

    public const string FRIDAY = "FRIDAY";

    public const string SATURDAY = "SATURDAY";

    public const string SUNDAY = "SUNDAY";


    public static readonly List<string> ALLDAYS =
    [

        MONDAY,
        TUESDAY,
        WEDNESDAY,
        THURSDAY,
        FRIDAY,
        SATURDAY,
        SUNDAY
    ];
}

public static class SlotStatusConstant
{
    public const string AVAILABLE = "AVAILABLE";
    public const string FULL = "FULL";
    public const string CANCELLED = "CANCELLED";
    public const string MAINTENANCE = "MAINTENANCE";
}