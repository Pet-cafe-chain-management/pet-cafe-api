using Microsoft.EntityFrameworkCore;
using PetCafe.Application.GlobalExceptionHandling.Exceptions;
using PetCafe.Application.Models.ShareModels;
using PetCafe.Domain.Entities;

namespace PetCafe.Application.Services;

public interface ISerOrderService
{
    Task<ServiceOrder> GetByOrderIdAsync(Guid orderId);
}


public class SerOrderService(IUnitOfWork _unitOfWork) : ISerOrderService
{
    public async Task<ServiceOrder> GetByOrderIdAsync(Guid orderId)
    {
        var serviceOrder = await _unitOfWork
         .ServiceOrderRepository
             .GetByIdAsync(orderId, includeFunc:
                x => x
                .Include(x => x.OrderDetails.Where(x => !x.IsDeleted)))
                ?? throw new BadRequestException("Không tìm thấy thông tin!");

        return serviceOrder;
    }
}