namespace PetCafe.Domain.Constants;

public static class DailyScheduleStatusConstant
{
    // Trạng thái chưa điểm danh
    public const string PENDING = "PENDING";

    // Trạng thái đã điểm danh - có mặt
    public const string PRESENT = "PRESENT";

    // Trạng thái đã điểm danh - vắng mặt
    public const string ABSENT = "ABSENT";

    // Trạng thái đã điểm danh - vắng có phép
    public const string EXCUSED = "EXCUSED";

    // Trạng thái đã điểm danh - đi muộn
    public const string LATE = "LATE";

    public static readonly List<string> ALL_STATUSES = [PENDING, PRESENT, ABSENT, EXCUSED, LATE];

}
