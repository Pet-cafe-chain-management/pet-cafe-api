using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.OrderModels;
using PetCafe.Application.Models.PayOsModels;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Application.Services.Commons;
using PetCafe.Application.Utilities;
using PetCafe.Domain.Constants;
using PetCafe.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace PetCafe.Application.Services;

public interface IOrderService
{
    Task<Order> CreateAsync(OrderCreateModel model);
    Task<bool> HandleWebhookAsync(WebhookResponseModel model);
    Task<Order> GetByIdAsync(Guid id);
    Task<Order> GetByOrderCodeAsync(double orderCode);
    Task<bool> ConfirmOrderAsync(Guid id);
    Task<BasePagingResponseModel<Order>> GetAllPagingAsync(OrderFilterQuery query, Guid? customerId);
    Task CleanupExpiredOrdersAsync();
}

public class OrderService(
    IUnitOfWork _unitOfWork,
    IClaimsService _claimsService,
    ICurrentTime _currentTime,
    IPayOsService _payOsService
) : IOrderService
{

    #region  Create Order
    public async Task<Order> CreateAsync(OrderCreateModel model)
    {
        var order = _unitOfWork.Mapper.Map<Order>(model);
        order.Type = _claimsService.GetCurrentUserRole == RoleConstants.EMPLOYEE ? OrderTypeConstant.EMPLOYEE : OrderTypeConstant.CUSTOMER;

        if (order.Type == OrderTypeConstant.EMPLOYEE && model.Products != null && model.Products.Count > 0)
        {
            var product_order = await CreateProductOrderDetail(order.Id, model.Products!);
            order.TotalAmount += product_order.FinalAmount;
        }

        if (model.Services != null && model.Services.Count > 0)
        {
            var service_order = await CreateServiceOrderDetail(order.Id, model.Services!);
            order.TotalAmount += service_order.FinalAmount;

        }

        order.OrderNumber = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds().ToString();
        order.FinalAmount = order.TotalAmount - order.DiscountAmount;

        if (order.PaymentMethod == PaymentMethodConstant.QR_CODE)
        {
            var payment = await _payOsService.CreatePaymentAsync(order.FinalAmount, Double.Parse(order.OrderNumber));
            order.PaymentDataJson = JsonConvert.SerializeObject(payment.Data);
        }
        await _unitOfWork.OrderRepository.AddAsync(order);

        await _unitOfWork.SaveChangesAsync();
        return order;
    }



    private async Task<ProductOrder> CreateProductOrderDetail(Guid orderId, List<ProductOrderModel> productModels)
    {
        var order_details = new List<ProductOrderDetail>();

        var product_order = new ProductOrder
        {
            OrderId = orderId,
            Status = OrderStatusConstant.PENDING,
        };

        foreach (var item in productModels)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId) ?? throw new Exception("Không tìm thấy thông tin sản phẩm");
            if (product.StockQuantity < item.Quantity) throw new BadRequestException($"Sản phẩm {product.Name} không đủ số lượng trong kho. Còn lại {product.StockQuantity}");
            if (!product.IsActive) throw new BadRequestException($"Sản phẩm {product.Name} hiện đang tạm ngưng phục vụ!");
            var detail = new ProductOrderDetail
            {
                ProductOrderId = product_order.Id,
                ProductId = item.ProductId,
                UnitPrice = product.Price,
                IsForFeeding = product.IsForPets,
                Notes = item.Notes,
                Quantity = item.Quantity,
                TotalPrice = product.Price * item.Quantity,
            };
            order_details.Add(detail);
        }
        product_order.TotalAmount = order_details.Sum(x => x.TotalPrice);
        product_order.FinalAmount = product_order.TotalAmount - product_order.DiscountAmount;

        await _unitOfWork.ProductOrderRepository.AddAsync(product_order);
        await _unitOfWork.ProductOrderDetailRepository.AddRangeAsync(order_details);

        return product_order;
    }

    private async Task<ServiceOrder> CreateServiceOrderDetail(Guid orderId, List<ServiceOrderModel> serviceModels)
    {
        var order_details = new List<ServiceOrderDetail>();

        var service_order = new ServiceOrder
        {
            OrderId = orderId,
            Status = OrderStatusConstant.PENDING,
            OrderDate = _currentTime.GetCurrentTime
        };

        // Begin transaction to ensure atomicity of slot reservation
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            foreach (var item in serviceModels)
            {
                var day_of_week = item.BookingDate.DayOfWeek.ToString().ToUpper();
                var slot = await _unitOfWork
                    .SlotRepository
                    .FirstOrDefaultAsync(x =>
                        x.Id == item.SlotId &&
                        x.ServiceStatus == SlotStatusConstant.AVAILABLE &&
                        x.Area.IsActive == true && x.Team.IsActive == true &&
                        x.Task.Status == TaskStatusConstant.ACTIVE &&
                        x.Area.IsDeleted == false && x.Team.IsDeleted == false && x.Service!.IsDeleted == false &&
                        ((x.IsRecurring && x.DayOfWeek != null && x.DayOfWeek.ToUpper() == day_of_week) ||
                        (!x.IsRecurring && x.SpecificDate != null && x.SpecificDate == item.BookingDate.Date)) &&
                        x.StartTime <= item.BookingDate.TimeOfDay &&
                        x.EndTime >= item.BookingDate.TimeOfDay,
                        includeFunc: x => x.Include(x => x.Service!).Include(x => x.Area).Include(x => x.Team).Include(x => x.Task))
                    ?? throw new BadRequestException("Dịch vụ hiện tại không khả dụng!");

                var bookingDate = DateOnly.FromDateTime(item.BookingDate.Date);

                // Reserve slot immediately when creating order to prevent race conditions
                // This ensures slot availability is checked and reserved atomically
                try
                {
                    await _unitOfWork.SlotAvailabilityRepository.IncrementBookedCountAsync(
                        item.SlotId,
                        bookingDate
                    );
                }
                catch (Exception ex)
                {
                    throw new BadRequestException($"Khung giờ hiện tại không còn chỗ trống! {ex.Message}");
                }

                var detail = new ServiceOrderDetail
                {
                    ServiceOrderId = service_order.Id,
                    ServiceId = slot.ServiceId,
                    SlotId = slot.Id,
                    UnitPrice = slot.Price,
                    Notes = item.Notes,
                    Quantity = 1,
                    TotalPrice = slot.Price,
                    BookingDate = item.BookingDate
                };
                order_details.Add(detail);
            }

            service_order.TotalAmount = order_details.Sum(x => x.TotalPrice);
            service_order.FinalAmount = service_order.TotalAmount - service_order.DiscountAmount;

            await _unitOfWork.ServiceOrderRepository.AddAsync(service_order);
            await _unitOfWork.ServiceOrderDetailRepository.AddRangeAsync(order_details);

            await _unitOfWork.CommitTransactionAsync();
            return service_order;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    #endregion

    public async Task<Order> GetByIdAsync(Guid id)
    {
        return await _unitOfWork.OrderRepository
            .GetByIdAsync(id,
                includeFunc: x => x
                    .Include(x => x.ProductOrder!).ThenInclude(x => x.OrderDetails.Where(x => !x.IsDeleted)).ThenInclude(x => x.Product)
                    .Include(x => x.ServiceOrder!).ThenInclude(x => x.OrderDetails.Where(x => !x.IsDeleted)).ThenInclude(x => x.Service)
                    .Include(x => x.Customer!)
                    .Include(x => x.Employee!)
                    .Include(x => x.Transactions)
            ) ?? throw new BadRequestException("Không tìm thấy thông tin!");
    }
    public async Task<Order> GetByOrderCodeAsync(double orderCode)
    {
        return await _unitOfWork.OrderRepository
            .FirstOrDefaultAsync(x => x.OrderNumber == orderCode.ToString(),
                includeFunc: x => x
                    .Include(x => x.ProductOrder!).ThenInclude(x => x.OrderDetails.Where(x => !x.IsDeleted)).ThenInclude(x => x.Product)
                    .Include(x => x.ServiceOrder!).ThenInclude(x => x.OrderDetails.Where(x => !x.IsDeleted)).ThenInclude(x => x.Service)
                    .Include(x => x.Customer!)
                    .Include(x => x.Employee!)
                    .Include(x => x.Transactions)
            ) ?? throw new BadRequestException("Không tìm thấy thông tin!");
    }

    #region Webhook handler
    public async Task<bool> HandleWebhookAsync(WebhookResponseModel model)
    {
        if (model.Code != "00") throw new Exception("Thanh toán thất bại!");
        var order = await _unitOfWork
            .OrderRepository
            .FirstOrDefaultAsync(x =>
                x.OrderNumber == model.Data!.OrderCode.ToString() ||
                model.Data!.Description!.Contains(x.OrderNumber)
            ) ?? throw new Exception("Thanh toán thất bại!");

        order.Status = OrderStatusConstant.PAID;
        order.PaymentStatus = PaymentStatusConstant.PAID;

        var transaction = _unitOfWork.Mapper.Map<Transaction>(model.Data);
        transaction.OrderId = order.Id;

        await UpdateServiceOrder(order.Id);

        await _unitOfWork.TransactionRepository.AddAsync(transaction);
        _unitOfWork.OrderRepository.Update(order);

        return await _unitOfWork.SaveChangesAsync();
    }


    private async Task UpdateProductOrder(Guid orderId)
    {
        var product_order = await _unitOfWork.ProductOrderRepository
            .FirstOrDefaultAsync(
                x => x.OrderId == orderId,
                includeFunc: x => x.Include(x => x.OrderDetails.Where(x => !x.IsDeleted)));

        if (product_order == null) return;

        foreach (var item in product_order.OrderDetails)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId!.Value) ?? throw new BadRequestException("Thanh toán thất bại!");

            if (product.StockQuantity < item.Quantity) throw new BadRequestException("Thanh toán thất bại!");

            product.StockQuantity -= item.Quantity;
            _unitOfWork.ProductRepository.Update(product);
        }

        product_order.Status = OrderStatusConstant.PAID;
        _unitOfWork.ProductOrderRepository.Update(product_order);

    }
    private async Task UpdateServiceOrder(Guid orderId)
    {
        var serivce_order = await _unitOfWork.ServiceOrderRepository
            .FirstOrDefaultAsync(x => x.OrderId == orderId,
                includeFunc: x => x.Include(x => x.OrderDetails.Where(x => !x.IsDeleted)));

        if (serivce_order == null) return;

        foreach (var item in serivce_order.OrderDetails.Where(x => x.SlotId != null))
        {
            var slot = await _unitOfWork.SlotRepository.GetByIdAsync(
                item.SlotId!.Value,
                includeFunc: x => x.Include(x => x.Service)
                                  .Include(x => x.Area)
                                  .Include(x => x.Team)) ?? throw new BadRequestException("Thanh toán thất bại!");

            // Slot was already reserved when order was created, so we don't need to increment again
            // Just create the CustomerBooking record
            await _unitOfWork.BookingRepository.AddAsync(
                new CustomerBooking
                {
                    SlotId = slot.Id,
                    OrderDetailId = item.Id,
                    ServiceId = slot.ServiceId!.Value,
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime,
                    BookingDate = item.BookingDate!.Value,
                    TeamId = slot.TeamId,
                    Notes = item.Notes,
                    BookingStatus = BookingStatusConstant.PENDING,
                }
            );
        }

        serivce_order.Status = OrderStatusConstant.PAID;
        _unitOfWork.ServiceOrderRepository.Update(serivce_order);
    }


    #endregion
    public async Task<bool> ConfirmOrderAsync(Guid id)
    {
        var order = await _unitOfWork
           .OrderRepository
           .GetByIdAsync(id) ?? throw new BadRequestException("Xác nhận thất bại!");

        if (order.Type != OrderTypeConstant.EMPLOYEE) throw new BadRequestException("Không có quyền xác nhận đơn hàng này!");
        order.Status = OrderStatusConstant.PAID;
        order.PaymentStatus = PaymentStatusConstant.PAID;

        await UpdateProductOrder(order.Id);
        await UpdateServiceOrder(order.Id);

        _unitOfWork.OrderRepository.Update(order);

        return await _unitOfWork.SaveChangesAsync();
    }

    #region GetAllPagingAsync

    public async Task<BasePagingResponseModel<Order>> GetAllPagingAsync(OrderFilterQuery query, Guid? customerId)
    {
        Expression<Func<Order, bool>>? filter = null;

        if (customerId != null && customerId != Guid.Empty)
        {
            Expression<Func<Order, bool>> filter_customer = x => x.CustomerId == customerId;
            filter = filter != null ? FilterCustoms.CombineFilters(filter, filter_customer) : filter_customer;
        }

        if (query.MinPrice != null || query.MaxPrice != null)
        {
            int min = query.MinPrice ?? 0;
            int max = query.MaxPrice ?? int.MaxValue;
            var filter_price = FilterByPrice(max, min);
            filter = filter != null ? FilterCustoms.CombineFilters(filter, filter_price) : filter_price;
        }

        if (query.StartDate != null || query.EndDate != null)
        {
            DateTime start = query.StartDate ?? DateTime.MinValue;
            DateTime end = query.EndDate ?? DateTime.MaxValue;
            var filter_date = FilterByDate(start, end);
            filter = filter != null ? FilterCustoms.CombineFilters(filter, filter_date) : filter_date;
        }

        if (!string.IsNullOrEmpty(query.Status))
        {
            var filter_status = FilterByStatus(query.Status);
            filter = filter != null ? FilterCustoms.CombineFilters(filter, filter_status) : filter_status;
        }

        if (!string.IsNullOrEmpty(query.Type))
        {
            var filter_type = FilterByType(query.Type);
            filter = filter != null ? FilterCustoms.CombineFilters(filter, filter_type) : filter_type;
        }

        if (!string.IsNullOrEmpty(query.PaymentMethod))
        {
            var filter_payment = FilterByPaymentMethod(query.PaymentMethod);
            filter = filter != null ? FilterCustoms.CombineFilters(filter, filter_payment) : filter_payment;
        }

        var (Pagination, Entities) = await _unitOfWork.OrderRepository.ToPagination(
            pageIndex: query.Page ?? 0,
            pageSize: query.Limit ?? 10,
            filter: filter,
            searchTerm: query.Q,
            searchFields: ["FullName", "Phone"],
            sortOrders: query.OrderBy?.ToDictionary(
                    k => k.OrderColumn ?? "CreatedAt",
                    v => (v.OrderDir ?? "ASC").Equals("ASC", StringComparison.CurrentCultureIgnoreCase)
                ) ?? new Dictionary<string, bool> { { "CreatedAt", false } },
            includeFunc: x => x.Include(x => x.Customer!).Include(x => x.Employee!).Include(x => x.ProductOrder!).Include(x => x.ServiceOrder!)
        );

        return BasePagingResponseModel<Order>.CreateInstance(Entities, Pagination); ;
    }

    private static Expression<Func<Order, bool>> FilterByPrice(int max, int min)
    {
        Expression<Func<Order, bool>> filter = x => x.FinalAmount >= min && x.FinalAmount <= max;
        return filter;
    }
    private static Expression<Func<Order, bool>> FilterByType(string type)
    {
        Expression<Func<Order, bool>> filter = x => x.Type == type;
        return filter;
    }
    private static Expression<Func<Order, bool>> FilterByPaymentMethod(string method)
    {
        Expression<Func<Order, bool>> filter = x => x.PaymentMethod == method;
        return filter;
    }
    private static Expression<Func<Order, bool>> FilterByStatus(string status)
    {
        Expression<Func<Order, bool>> filter = x => x.Status == status;
        return filter;
    }
    private static Expression<Func<Order, bool>> FilterByDate(DateTime startDate, DateTime endDate)
    {
        Expression<Func<Order, bool>> filter = x => x.OrderDate >= startDate && x.OrderDate <= endDate;
        return filter;
    }

    #endregion

    #region Cleanup Expired Orders

    public async Task CleanupExpiredOrdersAsync()
    {
        // Payment link expires after 15 minutes, so cleanup orders older than 15 minutes
        var expiredTime = _currentTime.GetCurrentTime.AddMinutes(-15);

        var expiredOrders = await _unitOfWork.OrderRepository
            .WhereAsync(x =>
                x.Status == OrderStatusConstant.PENDING &&
                x.PaymentStatus == PaymentStatusConstant.PENDING &&
                x.CreatedAt < expiredTime &&
                x.ServiceOrder != null);

        foreach (var order in expiredOrders)
        {
            try
            {
                // Release slot reservations for expired orders
                if (order.ServiceOrder != null)
                {
                    var serviceOrder = await _unitOfWork.ServiceOrderRepository
                        .FirstOrDefaultAsync(x => x.OrderId == order.Id,
                            includeFunc: x => x.Include(x => x.OrderDetails.Where(x => !x.IsDeleted)));

                    if (serviceOrder != null)
                    {
                        foreach (var detail in serviceOrder.OrderDetails.Where(x => x.SlotId.HasValue && x.BookingDate.HasValue))
                        {
                            var bookingDate = DateOnly.FromDateTime(detail.BookingDate!.Value.Date);
                            await _unitOfWork.SlotAvailabilityRepository
                                .DecrementBookedCountAsync(detail.SlotId!.Value, bookingDate);
                        }
                    }
                }

                // Mark order as expired
                order.Status = OrderStatusConstant.EXPIRED;
                _unitOfWork.OrderRepository.Update(order);
            }
            catch (Exception ex)
            {
                // Log error but continue processing other orders
                Console.WriteLine($"Error cleaning up order {order.Id}: {ex.Message}");
            }
        }

        if (expiredOrders.Count > 0)
        {
            await _unitOfWork.SaveChangesAsync();
        }
    }

    #endregion
}