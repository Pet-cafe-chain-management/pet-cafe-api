using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PetCafe.Application.Repositories;
using PetCafe.Application.Services.Commons;
using PetCafe.Domain.Entities;
using PetCafe.Domain.Models;

namespace PetCafe.Infrastructures.Repositories;

public class GenericRepository<TEntity>(
    AppDbContext context,
    ICurrentTime _timeService,
    IClaimsService _claimsService
) : IGenericRepository<TEntity> where TEntity : BaseEntity
{

    protected readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();
    #region Commons

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.CreatedAt = _timeService.GetCurrentTime;
        entity.CreatedBy = _claimsService.GetCurrentUser;
        var result = await _dbSet.AddAsync(entity, cancellationToken);
        return result.Entity;
    }

    public async Task AddRangeAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var currentTime = _timeService.GetCurrentTime;
        var currentUser = _claimsService.GetCurrentUser;

        foreach (var entity in entities)
        {
            entity.CreatedAt = currentTime;
            entity.CreatedBy = currentUser;
        }

        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public async Task<TEntity> EnsureExistsIfIdNotEmpty(Guid id)
    {
        if (id != Guid.Empty)
        {
            var entity = await GetByIdAsync(id) ?? throw new Exception($"{typeof(TEntity).Name} not found.");
            return entity;
        }
        return null!;
    }


    public void SoftRemove(TEntity entity)
    {
        entity.IsDeleted = true;
        entity.UpdatedAt = _timeService.GetCurrentTime;
        entity.UpdatedBy = _claimsService.GetCurrentUser;
        _dbSet.Update(entity);
    }


    public void SoftRemoveRange(List<TEntity> entities)
    {
        var currentTime = _timeService.GetCurrentTime;
        var currentUser = _claimsService.GetCurrentUser;

        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            entity.UpdatedAt = currentTime;
            entity.UpdatedBy = currentUser;
        }

        _dbSet.UpdateRange(entities);
    }

    public void Update(TEntity entity)
    {
        context.Entry(entity).State = EntityState.Detached;
        entity.UpdatedAt = _timeService.GetCurrentTime;
        entity.UpdatedBy = _claimsService.GetCurrentUser;
        _dbSet.Update(entity);
    }

    public void UpdateRange(List<TEntity> entities)
    {
        var currentTime = _timeService.GetCurrentTime;
        var currentUser = _claimsService.GetCurrentUser;

        foreach (var entity in entities)
        {
            context.Entry(entity).State = EntityState.Detached;
            entity.UpdatedAt = currentTime;
            entity.UpdatedBy = currentUser;
        }

        _dbSet.UpdateRange(entities);
    }

    #endregion

    #region  Querys
    public async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> expression,
        bool withDeleted = false,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeFunc = null,
        CancellationToken cancellationToken = default
    )
    {
        IQueryable<TEntity> query = _dbSet.AsQueryable();
        if (includeFunc != null)
        {
            query = includeFunc(query);
            query = query.AsSplitQuery();
        }
        query = GenericRepository<TEntity>.ApplyBaseFilters(query, withDeleted, expression);
        query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<TEntity>> GetAllAsync(
        bool withDeleted = false,
        List<(Expression<Func<TEntity, object>> OrderBy, bool IsDescending)>? orderByList = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeFunc = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = PrepareQuery(withDeleted, null, orderByList, true, includeFunc);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(
        Guid id,
        bool withDeleted = false,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeFunc = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = ApplyIncludes(_dbSet.AsQueryable(), includeFunc).AsNoTracking();
        return await query.FirstOrDefaultAsync(
            x => x.Id.Equals(id) && (withDeleted || !x.IsDeleted),
            cancellationToken);
    }

    public async Task<(Pagination Pagination, List<TEntity> Entities)> ToPagination(
        int pageIndex = 0,
        int pageSize = 10,
        bool withDeleted = false,
        string? searchTerm = null,
        List<string>? searchFields = null,
        Dictionary<string, bool>? sortOrders = null,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeFunc = null,
        CancellationToken cancellationToken = default
    )
    {
        pageIndex = Math.Max(0, pageIndex);
        pageSize = Math.Max(1, pageSize);

        IQueryable<TEntity> query = _dbSet.AsQueryable();

        query = PrepareQuery(withDeleted, filter, null, true, includeFunc);

        if (!string.IsNullOrWhiteSpace(searchTerm) && searchFields != null)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            Expression? combined = null;

            foreach (var field in searchFields)
            {
                var property = Expression.Property(parameter, field);
                if (property.Type != typeof(string)) continue;

                // Check if property is not null
                var notNullCheck = Expression.NotEqual(property, Expression.Constant(null));

                // Use Contains with StringComparison.OrdinalIgnoreCase
                var containsMethod = typeof(string).GetMethod("Contains", [typeof(string), typeof(StringComparison)])!;
                var containsCall = Expression.Call(
                   property,
                   typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!,
                   Expression.Constant(searchTerm)
                );

                var safeContains = Expression.AndAlso(notNullCheck, containsCall);
                combined = combined == null ? safeContains : Expression.OrElse(combined, safeContains);
            }

            if (combined != null)
            {
                var lambda = Expression.Lambda<Func<TEntity, bool>>(combined, parameter);
                query = query.Where(lambda);
            }
        }

        if (sortOrders != null && sortOrders.Count != 0)
        {
            IOrderedQueryable<TEntity>? orderedQuery = null;

            foreach (var sort in sortOrders)
            {
                try
                {
                    var pascalKey = SnakeToPascal(sort.Key);
                    var parameter = Expression.Parameter(typeof(TEntity), "x");
                    var property = Expression.Property(parameter, pascalKey);
                    var converted = Expression.Convert(property, typeof(object));
                    var lambda = Expression.Lambda<Func<TEntity, object>>(converted, parameter);
                    if (orderedQuery == null)
                    {
                        orderedQuery = sort.Value
                            ? query.OrderByDescending(lambda)
                            : query.OrderBy(lambda);
                    }
                    else
                    {
                        orderedQuery = sort.Value
                            ? orderedQuery.ThenByDescending(lambda)
                            : orderedQuery.ThenBy(lambda);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }

            query = orderedQuery ?? query;
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var pagination = new Pagination
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalItemsCount = totalCount,
        };

        return (pagination, items);

    }

    public async Task<List<TEntity>> WhereAsync(
        Expression<Func<TEntity, bool>> filter,
        bool withDeleted = false,
        List<(Expression<Func<TEntity, object>> OrderBy, bool IsDescending)>? orderByList = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeFunc = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = PrepareQuery(withDeleted, filter, orderByList, true, includeFunc);
        return await query.ToListAsync(cancellationToken);
    }
    #endregion

    #region Query Helpers

    private static string SnakeToPascal(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var parts = input.Split('_');
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i].Length > 0)
                parts[i] = char.ToUpper(parts[i][0]) + parts[i][1..];
        }
        return string.Concat(parts);
    }

    private static IQueryable<TEntity> ApplyBaseFilters(
        IQueryable<TEntity> query,
        bool withDeleted = false,
        Expression<Func<TEntity, bool>>? filter = null)
    {
        if (!withDeleted)
        {
            query = query.Where(x => !x.IsDeleted);
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return query;
    }

    private static IQueryable<TEntity> ApplyIncludes(
        IQueryable<TEntity> query,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeFunc = null)
    {
        if (includeFunc != null)
        {
            query = includeFunc(query);
            query = query.AsSplitQuery();
        }

        return query;
    }

    private static IQueryable<TEntity> ApplyOrdering(
        IQueryable<TEntity> query,
        List<(Expression<Func<TEntity, object>> OrderBy, bool IsDescending)>? orderByList = null)
    {
        if (orderByList == null || orderByList.Count == 0)
        {
            return query.OrderByDescending(x => x.CreatedAt);
        }

        var isFirstOrder = true;
        IOrderedQueryable<TEntity>? orderedQuery = null;

        foreach (var (orderBy, isDescending) in orderByList)
        {
            if (isFirstOrder)
            {
                orderedQuery = isDescending
                    ? query.OrderByDescending(orderBy)
                    : query.OrderBy(orderBy);
                isFirstOrder = false;
            }
            else
            {
                orderedQuery = isDescending
                    ? orderedQuery!.ThenByDescending(orderBy)
                    : orderedQuery!.ThenBy(orderBy);
            }
        }

        return orderedQuery!;
    }


    private IQueryable<TEntity> PrepareQuery(
        bool withDeleted = false,
        Expression<Func<TEntity, bool>>? filter = null,
        List<(Expression<Func<TEntity, object>> OrderBy, bool IsDescending)>? orderByList = null,
        bool asNoTracking = true,
       Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeFunc = null)
    {
        var query = _dbSet.AsQueryable();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        query = GenericRepository<TEntity>.ApplyIncludes(query, includeFunc);
        query = GenericRepository<TEntity>.ApplyBaseFilters(query, withDeleted, filter);
        query = GenericRepository<TEntity>.ApplyOrdering(query, orderByList);

        return query;
    }

    #endregion
}