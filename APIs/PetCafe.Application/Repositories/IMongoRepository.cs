using System.Linq.Expressions;
using PetCafe.Domain.Entities;
using PetCafe.Domain.Models;
using Task = System.Threading.Tasks.Task;

namespace PetCafe.Application.Repositories;

public interface IMongoRepository<TEntity> where TEntity : BaseEntity
{
    #region Query Methods

    Task<List<TEntity>> GetAllAsync(
        bool withDeleted = false,
        List<(Expression<Func<TEntity, object>> OrderBy, bool IsDescending)>? orderByList = null,
        CancellationToken cancellationToken = default);

    Task<TEntity?> GetByIdAsync(
        Guid id,
        bool withDeleted = false,
        CancellationToken cancellationToken = default
    );

    Task<List<TEntity>> WhereAsync(
        Expression<Func<TEntity, bool>> filter,
        bool withDeleted = false,
        List<(Expression<Func<TEntity, object>> OrderBy, bool IsDescending)>? orderByList = null,
        CancellationToken cancellationToken = default);

    Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> expression,
        bool withDeleted = false,
        CancellationToken cancellationToken = default
    );

    Task<long> CountAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        bool withDeleted = false,
        CancellationToken cancellationToken = default);

    #endregion

    #region Command Methods

    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync(List<TEntity> entities, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<bool> UpdateRangeAsync(List<TEntity> entities, CancellationToken cancellationToken = default);

    Task<bool> SoftRemoveAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<bool> SoftRemoveRangeAsync(List<TEntity> entities, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<bool> DeleteRangeAsync(List<TEntity> entities, CancellationToken cancellationToken = default);

    Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default);

    #endregion

    #region Pagination Methods

    Task<(Pagination Pagination, List<TEntity> Entities)> ToPaginationAsync(
        int pageIndex = 0,
        int pageSize = 10,
        bool withDeleted = false,
        string? searchTerm = null,
        List<string>? searchFields = null,
        Dictionary<string, bool>? sortOrders = null,
        Expression<Func<TEntity, bool>>? filter = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Generic Function
    Task<TEntity> EnsureExistsIfIdNotEmpty(Guid id, CancellationToken cancellationToken = default);
    #endregion
}

