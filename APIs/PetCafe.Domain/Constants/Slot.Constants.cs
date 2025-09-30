namespace PetCafe.Domain.Constants;

public static class DayConstant
{

    public const string MONDAY = "Monday";

    public const string TUESDAY = "Tuesday";

    public const string WEDNESDAY = "Wednesday";

    public const string THURSDAY = "Thursday";

    public const string FRIDAY = "Friday";

    public const string SATURDAY = "Saturday";

    public const string SUNDAY = "Sunday";


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