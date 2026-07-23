using Microsoft.EntityFrameworkCore;
using PowerManager.Server.Entity;
using System.Linq.Expressions;

namespace PowerManager.Server.Context.Store
{
    public interface IReadStore<TContext, TEntity> where TContext : DbContext
    {
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken);

        Task<bool> AnyAsync(TContext context, Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken);

        ValueTask<TEntity?> FindAsync(object?[]? keyValues, CancellationToken cancellationToken);

        ValueTask<TEntity?> FindAsync(TContext context, object?[]? keyValues, CancellationToken cancellationToken);

        Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken);

        Task<List<TEntity>> GetListAsync(TContext context, CancellationToken cancellationToken);

        Task<PagedResult<TEntity>> GetListAsync(SearchQuery query, CancellationToken cancellationToken);

        Task<PagedResult<TEntity>> GetListAsync(TContext context, SearchQuery query, CancellationToken cancellationToken);
    }

    public interface IStore<TContext, TEntity> : IReadStore<TContext, TEntity> where TContext : DbContext
    {
        Task CreateAsync(TEntity entity, CancellationToken cancellationToken);

        Task CreateAsync(TContext context, TEntity entity, CancellationToken cancellationToken);

        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);

        Task UpdateAsync(TContext context, TEntity entity, CancellationToken cancellationToken);

        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken);

        Task DeleteAsync(TContext context, TEntity entity, CancellationToken cancellationToken);
    }
}
