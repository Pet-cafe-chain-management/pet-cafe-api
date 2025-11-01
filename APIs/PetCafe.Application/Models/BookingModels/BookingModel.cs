using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Domain.Constants;

namespace PetCafe.Application.Models.BookingModels;

public class BookingUpdateModel
{
    public string BookingStatus { get; set; } = BookingStatusConstant.PENDING;
    public string? Notes { get; set; }
    public string? CancelReason { get; set; }

}

public class BookingUpdateModelValidator : AbstractValidator<BookingUpdateModel>
{
    public BookingUpdateModelValidator()
    {
        RuleFor(x => x.BookingStatus)
            .NotEmpty().WithMessage("Trạng thái đặt lịch không được để trống")
            .Must(status => BookingStatusConstant.ALL_STATUSES.Contains(status)).WithMessage("Trạng thái đặt lịch không hợp lệ");
        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Ghi chú không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Notes));


        RuleFor(x => x.CancelReason)
            .MaximumLength(500).WithMessage("Lý do hủy đặt lịch không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.CancelReason));


    }
}

public class BookingFilterQuery : FilterQuery
{
    [FromQuery(Name = "booking_status")]
    public string? BookingStatus { get; set; }
    [FromQuery(Name = "from_date")]
    public DateTime? FromDate { get; set; }
    [FromQuery(Name = "to_date")]
    public DateTime? ToDate { get; set; }

    [FromQuery(Name = "team_id")]
    public Guid? TeamId { get; set; }

    [FromQuery(Name = "service_id")]
    public Guid? ServiceId { get; set; }

    [FromQuery(Name = "customer_id")]
    public Guid? CustomerId { get; set; }
}