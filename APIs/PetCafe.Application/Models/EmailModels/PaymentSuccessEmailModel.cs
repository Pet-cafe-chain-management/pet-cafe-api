namespace PetCafe.Application.Models.EmailModels;

public class PaymentSuccessEmailModel
{
    public string CustomerName { get; set; } = default!;
    public string CustomerEmail { get; set; } = default!;
    public string? CustomerPhone { get; set; }
    public string OrderNumber { get; set; } = default!;
    public double TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }
    public List<BookingTimeInfo>? BookingTimes { get; set; }
}

public class BookingTimeInfo
{
    public string ServiceName { get; set; } = default!;
    public DateTime BookingDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}


