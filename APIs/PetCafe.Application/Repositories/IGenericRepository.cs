using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using PetCafe.Domain.Entities;
using PetCafe.Domain.Models;

namespace PetCafe.Application.Repositories;

public interface IGenericRepository<TEntity> where TEntity : BaseEntity
{
    #region Query Methods

    Task<List<TEntity>> GetAllAsync(
        bool withDeleted = false,
        List<(Expression<Func<TEntity, object>> OrderBy, bool IsDescending)>? orderByList = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeFunc = null,
        CancellationToken cancellationToken = default);

    Task<TEntity?> GetByIdAsync(
        Guid id,
        bool withDeleted = false,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeFunc = null,
        CancellationToken cancellationToken = default
    );



    Task<List<TEntity>> WhereAsync(
        Expression<Func<TEntity, bool>> filter,
        bool withDeleted = false,
        List<(Expression<Func<TEntity, object>> OrderBy, bool IsDescending)>? orderByList = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeFunc = null,
        CancellationToken cancellationToken = default);

    Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> expression,
        bool withDeleted = false,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeFunc = null,
        CancellationToken cancellationToken = default
    );


    #endregion

    #region Command Methods

    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);


    Task AddRangeAsync(List<TEntity> entities, CancellationToken cancellationToken = default);


    void Update(TEntity entity);


    void UpdateRange(List<TEntity> entities);


    void SoftRemove(TEntity entity);


    void SoftRemoveRange(List<TEntity> entities);

    #endregion

    #region Pagination Methods

    public Task<(Pagination Pagination, List<TEntity> Entities)> ToPagination(
        int pageIndex = 0,
        int pageSize = 10,
        bool withDeleted = false,
        string? searchTerm = null,
        List<string>? searchFields = null,
        Dictionary<string, bool>? sortOrders = null,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeFunc = null,
        CancellationToken cancellationToken = default);
    #endregion

    #region Generic Function
    Task<TEntity> EnsureExistsIfIdNotEmpty(Guid id);
    #endregion
}

