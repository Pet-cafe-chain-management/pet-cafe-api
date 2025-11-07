namespace PetCafe.Domain.Constants;

public static class OrderTypeConstant
{
    public const string CUSTOMER = "CUSTOMER";
    public const string EMPLOYEE = "EMPLOYEE";
}


public static class OrderStatusConstant
{

    public const string PENDING = "PENDING";

    public const string EXPIRED = "EXPIRED";

    public const string PAID = "PAID";

    public const string PROCESSING = "PROCESSING";

    public const string COMPLETED = "COMPLETED";
}

public static class PaymentMethodConstant
{
    public const string CASH = "CASH";
    public const string QR_CODE = "QR_CODE";
}

public static class PaymentStatusConstant
{
    public const string PENDING = "PENDING";
    public const string PAID = "PAID";
}