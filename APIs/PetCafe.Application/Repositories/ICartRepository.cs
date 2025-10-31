using PetCafe.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace PetCafe.Application.Repositories;

public interface ICartRepository : IMongoRepository<Cart>
{
    Task<Cart?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    
    Task<bool> DeleteByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
}

