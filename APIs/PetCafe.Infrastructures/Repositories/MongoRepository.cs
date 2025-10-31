using System.Linq.Expressions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using PetCafe.Application.Repositories;
using PetCafe.Application.Services.Commons;
using PetCafe.Domain.Entities;
using PetCafe.Domain.Models;
using Task = System.Threading.Tasks.Task;

namespace PetCafe.Infrastructures.Repositories;

public class MongoRepository<TEntity>(
    MongoDbContext mongoDbContext,
    ICurrentTime timeService,
    IClaimsService claimsService) : IMongoRepository<TEntity> where TEntity : BaseEntity
{
    private readonly IMongoCollection<TEntity> _collection = mongoDbContext.GetCollection<TEntity>();

    #region Query Methods

    public async Task<List<TEntity>> GetAllAsync(
        bool withDeleted = false,
        List<(Expression<Func<TEntity, object>> OrderBy, bool IsDescending)>? orderByList = null,
        CancellationToken cancellationToken = default)
    {
        var query = _collection.AsQueryable();

        if (!withDeleted)
        {
            query = query.Where(x => !x.IsDeleted);
        }

        query = ApplyOrdering(query, orderByList);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(
        Guid id,
        bool withDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var filterBuilder = Builders<TEntity>.Filter;
        var filter = filterBuilder.Eq(x => x.Id, id);

        if (!withDeleted)
        {
            filter &= filterBuilder.Eq(x => x.IsDeleted, false);
        }

        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<TEntity>> WhereAsync(
        Expression<Func<TEntity, bool>> filter,
        bool withDeleted = false,
        List<(Expression<Func<TEntity, object>> OrderBy, bool IsDescending)>? orderByList = null,
        CancellationToken cancellationToken = default)
    {
        var query = _collection.AsQueryable().Where(filter);

        if (!withDeleted)
        {
            query = query.Where(x => !x.IsDeleted);
        }

        query = ApplyOrdering(query, orderByList);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> expression,
        bool withDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var query = _collection.AsQueryable().Where(expression);

        if (!withDeleted)
        {
            query = query.Where(x => !x.IsDeleted);
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<long> CountAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        bool withDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var query = _collection.AsQueryable();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (!withDeleted)
        {
            query = query.Where(x => !x.IsDeleted);
        }

        return await query.CountAsync(cancellationToken);
    }

    #endregion

    #region Command Methods

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = timeService.GetCurrentTime;
        entity.CreatedBy = claimsService.GetCurrentUser;
        entity.IsDeleted = false;

        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
        return entity;
    }

    public async Task AddRangeAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var currentTime = timeService.GetCurrentTime;
        var currentUser = claimsService.GetCurrentUser;

        foreach (var entity in entities)
        {
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = currentTime;
            entity.CreatedBy = currentUser;
            entity.IsDeleted = false;
        }

        if (entities.Count > 0)
        {
            await _collection.InsertManyAsync(entities, cancellationToken: cancellationToken);
        }
    }

    public async Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.UpdatedAt = timeService.GetCurrentTime;
        entity.UpdatedBy = claimsService.GetCurrentUser;

        var filter = Builders<TEntity>.Filter.Eq(x => x.Id, entity.Id);
        var result = await _collection.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> UpdateRangeAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var currentTime = timeService.GetCurrentTime;
        var currentUser = claimsService.GetCurrentUser;
        var writeModels = new List<ReplaceOneModel<TEntity>>();

        foreach (var entity in entities)
        {
            entity.UpdatedAt = currentTime;
            entity.UpdatedBy = currentUser;

            var filter = Builders<TEntity>.Filter.Eq(x => x.Id, entity.Id);
            writeModels.Add(new ReplaceOneModel<TEntity>(filter, entity) { IsUpsert = false });
        }

        if (writeModels.Count > 0)
        {
            var result = await _collection.BulkWriteAsync(writeModels, cancellationToken: cancellationToken);
            return result.ModifiedCount > 0;
        }

        return false;
    }

    public async Task<bool> SoftRemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.IsDeleted = true;
        entity.UpdatedAt = timeService.GetCurrentTime;
        entity.UpdatedBy = claimsService.GetCurrentUser;

        var filter = Builders<TEntity>.Filter.Eq(x => x.Id, entity.Id);
        var update = Builders<TEntity>.Update
            .Set(x => x.IsDeleted, true)
            .Set(x => x.UpdatedAt, entity.UpdatedAt)
            .Set(x => x.UpdatedBy, entity.UpdatedBy);

        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> SoftRemoveRangeAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var currentTime = timeService.GetCurrentTime;
        var currentUser = claimsService.GetCurrentUser;
        var writeModels = new List<UpdateOneModel<TEntity>>();

        foreach (var entity in entities)
        {
            var filter = Builders<TEntity>.Filter.Eq(x => x.Id, entity.Id);
            var update = Builders<TEntity>.Update
                .Set(x => x.IsDeleted, true)
                .Set(x => x.UpdatedAt, currentTime)
                .Set(x => x.UpdatedBy, currentUser);

            writeModels.Add(new UpdateOneModel<TEntity>(filter, update));
        }

        if (writeModels.Count > 0)
        {
            var result = await _collection.BulkWriteAsync(writeModels, cancellationToken: cancellationToken);
            return result.ModifiedCount > 0;
        }

        return false;
    }

    public async Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TEntity>.Filter.Eq(x => x.Id, entity.Id);
        var result = await _collection.DeleteOneAsync(filter, cancellationToken);
        return result.DeletedCount > 0;
    }

    public async Task<bool> DeleteRangeAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var ids = entities.Select(x => x.Id).ToList();
        var filter = Builders<TEntity>.Filter.In(x => x.Id, ids);
        var result = await _collection.DeleteManyAsync(filter, cancellationToken);
        return result.DeletedCount > 0;
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TEntity>.Filter.Eq(x => x.Id, id);
        var result = await _collection.DeleteOneAsync(filter, cancellationToken);
        return result.DeletedCount > 0;
    }

    #endregion

    #region Pagination Methods

    public async Task<(Pagination Pagination, List<TEntity> Entities)> ToPaginationAsync(
        int pageIndex = 0,
        int pageSize = 10,
        bool withDeleted = false,
        string? searchTerm = null,
        List<string>? searchFields = null,
        Dictionary<string, bool>? sortOrders = null,
        Expression<Func<TEntity, bool>>? filter = null,
        CancellationToken cancellationToken = default)
    {
        pageIndex = Math.Max(0, pageIndex);
        pageSize = Math.Max(1, pageSize);

        var query = _collection.AsQueryable();

        // Apply filter
        if (filter != null)
        {
            query = query.Where(filter);
        }

        // Apply soft delete filter
        if (!withDeleted)
        {
            query = query.Where(x => !x.IsDeleted);
        }

        // Apply search
        if (!string.IsNullOrWhiteSpace(searchTerm) && searchFields != null && searchFields.Count > 0)
        {
            query = ApplySearch(query, searchTerm, searchFields);
        }

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = ApplySorting(query, sortOrders);

        // Apply pagination
        var items = await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var pagination = new Pagination
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalItemsCount = (int)totalCount,
        };

        return (pagination, items);
    }

    #endregion

    #region Helper Methods

    private static IQueryable<TEntity> ApplyOrdering(
        IQueryable<TEntity> query,
        List<(Expression<Func<TEntity, object>> OrderBy, bool IsDescending)>? orderByList)
    {
        if (orderByList == null || orderByList.Count == 0)
        {
            return query.OrderByDescending(x => x.CreatedAt);
        }

        IOrderedQueryable<TEntity>? orderedQuery = null;
        var isFirstOrder = true;

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

        return orderedQuery ?? query;
    }

    private static IQueryable<TEntity> ApplySorting(
        IQueryable<TEntity> query,
        Dictionary<string, bool>? sortOrders)
    {
        if (sortOrders == null || sortOrders.Count == 0)
        {
            return query.OrderByDescending(x => x.CreatedAt);
        }

        IOrderedQueryable<TEntity>? orderedQuery = null;
        var isFirstOrder = true;

        foreach (var sort in sortOrders)
        {
            try
            {
                var pascalKey = SnakeToPascal(sort.Key);
                var parameter = Expression.Parameter(typeof(TEntity), "x");
                var property = Expression.Property(parameter, pascalKey);
                var converted = Expression.Convert(property, typeof(object));
                var lambda = Expression.Lambda<Func<TEntity, object>>(converted, parameter);

                if (isFirstOrder)
                {
                    orderedQuery = sort.Value
                        ? query.OrderByDescending(lambda)
                        : query.OrderBy(lambda);
                    isFirstOrder = false;
                }
                else
                {
                    orderedQuery = sort.Value
                        ? orderedQuery!.ThenByDescending(lambda)
                        : orderedQuery!.ThenBy(lambda);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying sort for {sort.Key}: {ex.Message}");
            }
        }

        return orderedQuery ?? query;
    }

    private static IQueryable<TEntity> ApplySearch(
        IQueryable<TEntity> query,
        string searchTerm,
        List<string> searchFields)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "x");
        Expression? combined = null;

        foreach (var field in searchFields)
        {
            try
            {
                var pascalKey = SnakeToPascal(field);
                var property = Expression.Property(parameter, pascalKey);

                if (property.Type != typeof(string)) continue;

                // Check if property is not null
                var notNullCheck = Expression.NotEqual(property, Expression.Constant(null));

                // Use Contains
                var containsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;
                var containsCall = Expression.Call(
                    property,
                    containsMethod,
                    Expression.Constant(searchTerm)
                );

                var safeContains = Expression.AndAlso(notNullCheck, containsCall);
                combined = combined == null ? safeContains : Expression.OrElse(combined, safeContains);
            }
            catch
            {
                // Skip invalid field
                continue;
            }
        }

        if (combined != null)
        {
            var lambda = Expression.Lambda<Func<TEntity, bool>>(combined, parameter);
            query = query.Where(lambda);
        }

        return query;
    }

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

    #endregion

    #region Generic Function

    public async Task<TEntity> EnsureExistsIfIdNotEmpty(Guid id, CancellationToken cancellationToken = default)
    {
        if (id != Guid.Empty)
        {
            var entity = await GetByIdAsync(id, cancellationToken: cancellationToken) 
                ?? throw new Exception($"{typeof(TEntity).Name} not found.");
            return entity;
        }
        return null!;
    }

    #endregion
}

