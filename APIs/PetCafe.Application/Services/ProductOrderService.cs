using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Domain.Entities;

namespace PetCafe.Application.Services;

public interface IProductOrderService
{
    Task<ProductOrder> GetByOrderIdAsync(Guid orderId);
}


public class ProductOrderService(IUnitOfWork _unitOfWork) : IProductOrderService
{
    public async Task<ProductOrder> GetByOrderIdAsync(Guid orderId)
    {
        var productOrder = await _unitOfWork
            .ProductOrderRepository
                .GetByIdAsync(orderId, includeFunc:
                    x => x.Include(x => x.OrderDetails.Where(x => !x.IsDeleted)))
                    ?? throw new BadRequestException("Không tìm thấy thông tin!");

        return productOrder;
    }
}