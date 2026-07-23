using Microsoft.EntityFrameworkCore;
using PowerManager.Server.Context.Entity;
using PowerManager.Server.Entity;
using System.Linq.Expressions;

namespace PowerManager.Server.Context.Store
{
    public sealed class ElectricPowerStore(IDbContextFactory<PowerManagerContext> dbContextFactory) : IReadStore<PowerManagerContext, ElectricPower>
    {
        public async Task<bool> AnyAsync(Expression<Func<ElectricPower, bool>> expression, CancellationToken cancellationToken)
        {
            using PowerManagerContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            return await AnyAsync(context, expression, cancellationToken);
        }

        public async Task<bool> AnyAsync(PowerManagerContext context, Expression<Func<ElectricPower, bool>> expression, CancellationToken cancellationToken)
        {
            return await context.ElectricPower.AnyAsync(expression, cancellationToken);
        }

        public async ValueTask<ElectricPower?> FindAsync(object?[]? keyValues, CancellationToken cancellationToken)
        {
            using PowerManagerContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            return await FindAsync(context, keyValues, cancellationToken);
        }

        public async ValueTask<ElectricPower?> FindAsync(PowerManagerContext context, object?[]? keyValues, CancellationToken cancellationToken)
        {
            return await context.ElectricPower.FindAsync(keyValues, cancellationToken);
        }

        public async Task<List<ElectricPower>> GetListAsync(CancellationToken cancellationToken)
        {
            using PowerManagerContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            return await GetListAsync(context, cancellationToken);
        }

        public async Task<List<ElectricPower>> GetListAsync(PowerManagerContext context, CancellationToken cancellationToken)
        {
            return await context.ElectricPower.ToListAsync(cancellationToken);
        }

        public async Task<PagedResult<ElectricPower>> GetListAsync(SearchQuery query, CancellationToken cancellationToken)
        {
            using PowerManagerContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            return await GetListAsync(context, query, cancellationToken);
        }

        public async Task<PagedResult<ElectricPower>> GetListAsync(PowerManagerContext context, SearchQuery query, CancellationToken cancellationToken)
        {
            IQueryable<ElectricPower> filtered = context.ElectricPower
                .Where(entity => string.IsNullOrWhiteSpace(query.Query) || entity.PowerTypeName.Contains(query.Query));

            int totalCount = await filtered.CountAsync(cancellationToken);
            List<ElectricPower> items = await filtered
                .OrderBy(entity => entity.PowerType)
                .Skip((query.Pagination.CurrentPage - 1) * query.Pagination.ItemSize)
                .Take(query.Pagination.ItemSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<ElectricPower>
            {
                Items = items,
                Pagination = new Pagination
                {
                    CurrentPage = query.Pagination.CurrentPage,
                    ItemSize = query.Pagination.ItemSize,
                    TotalCount = totalCount,
                },
            };
        }
    }
}
