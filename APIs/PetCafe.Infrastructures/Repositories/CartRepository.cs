using MongoDB.Driver;
using MongoDB.Driver.Linq;
using PetCafe.Application.Repositories;
using PetCafe.Application.Services.Commons;
using PetCafe.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace PetCafe.Infrastructures.Repositories;

public class CartRepository(
    MongoDbContext mongoDbContext,
    ICurrentTime timeService,
    IClaimsService claimsService) : MongoRepository<Cart>(mongoDbContext, timeService, claimsService), ICartRepository
{
    private readonly IMongoCollection<Cart> _collection = mongoDbContext.GetCollection<Cart>();

    public async Task<Cart?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var query = _collection.AsQueryable()
            .Where(x => x.CustomerId == customerId && !x.IsDeleted);
        
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> DeleteByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Cart>.Filter.Eq(x => x.CustomerId, customerId);
        var result = await _collection.DeleteOneAsync(filter, cancellationToken);
        return result.DeletedCount > 0;
    }
}

