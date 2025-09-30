using PetCafe.Application.Models.OrderModels;
using PetCafe.Application.Services.Commons;
using PetCafe.Domain.Entities;

namespace PetCafe.Application.Services;

public interface IOrderService
{
    Task<Order> CreateAsync(OrderCreateModel model);
}

public class OrderService(
    IUnitOfWork _unitOfWork,
    IClaimsService _claimsService
) : IOrderService
{
    public Task<Order> CreateAsync(OrderCreateModel model)
    {
        var order = _unitOfWork.Mapper.Map<Order>(model);
        // order.Type = 
        throw new NotImplementedException();
    }
}