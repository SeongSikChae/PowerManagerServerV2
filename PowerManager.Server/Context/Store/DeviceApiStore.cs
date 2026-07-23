using Microsoft.EntityFrameworkCore;
using PowerManager.Server.Context.Entity;
using PowerManager.Server.Entity;
using System.Linq.Expressions;

namespace PowerManager.Server.Context.Store
{
    public sealed class DeviceApiStore(IDbContextFactory<PowerManagerContext> dbContextFactory) : IStore<PowerManagerContext, DeviceApi>
    {
        public async Task<bool> AnyAsync(Expression<Func<DeviceApi, bool>> expression, CancellationToken cancellationToken)
        {
            using PowerManagerContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            return await AnyAsync(context, expression, cancellationToken);
        }

        public async Task<bool> AnyAsync(PowerManagerContext context, Expression<Func<DeviceApi, bool>> expression, CancellationToken cancellationToken)
        {
            return await context.DeviceApi.AnyAsync(expression, cancellationToken);
        }

        public async ValueTask<DeviceApi?> FindAsync(object?[]? keyValues, CancellationToken cancellationToken)
        {
            using PowerManagerContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            return await FindAsync(context, keyValues, cancellationToken);
        }

        public async ValueTask<DeviceApi?> FindAsync(PowerManagerContext context, object?[]? keyValues, CancellationToken cancellationToken)
        {
            return await context.DeviceApi.FindAsync(keyValues, cancellationToken);
        }

        public async Task<List<DeviceApi>> GetListAsync(CancellationToken cancellationToken)
        {
            using PowerManagerContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            return await GetListAsync(context, cancellationToken);
        }

        public async Task<List<DeviceApi>> GetListAsync(PowerManagerContext context, CancellationToken cancellationToken)
        {
            return await context.DeviceApi.ToListAsync(cancellationToken);
        }

        public async Task<PagedResult<DeviceApi>> GetListAsync(SearchQuery query, CancellationToken cancellationToken)
        {
            using PowerManagerContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            return await GetListAsync(context, query, cancellationToken);
        }

        public async Task<PagedResult<DeviceApi>> GetListAsync(PowerManagerContext context, SearchQuery query, CancellationToken cancellationToken)
        {
            IQueryable<DeviceApi> filtered = context.DeviceApi
                .Where(entity => string.IsNullOrWhiteSpace(query.Query) || entity.UserId.Equals(query.Query) || entity.DeviceId.Equals(query.Query));

            int totalCount = await filtered.CountAsync(cancellationToken);
            List<DeviceApi> items = await filtered
                .OrderBy(entity => entity.UserId)
                .OrderBy(entity => entity.DeviceId)
                .Skip((query.Pagination.CurrentPage - 1) * query.Pagination.ItemSize)
                .Take(query.Pagination.ItemSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<DeviceApi>
            {
                Items = items,
                Pagination = new Pagination
                {
                    CurrentPage = query.Pagination.CurrentPage,
                    ItemSize = query.Pagination.ItemSize,
                    TotalCount = totalCount
                },
            };
        }

        public async Task CreateAsync(DeviceApi entity, CancellationToken cancellationToken)
        {
            using PowerManagerContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            await CreateAsync(context, entity, cancellationToken);
        }

        public async Task CreateAsync(PowerManagerContext context, DeviceApi entity, CancellationToken cancellationToken)
        {
            await context.DeviceApi.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(DeviceApi entity, CancellationToken cancellationToken)
        {
            using PowerManagerContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            await UpdateAsync(context, entity, cancellationToken);
        }

        public async Task UpdateAsync(PowerManagerContext context, DeviceApi entity, CancellationToken cancellationToken)
        {
            await context.DeviceApi
                .Where(e => e.UserId.Equals(entity.UserId) && e.DeviceId.Equals(entity.DeviceId))
                .ExecuteUpdateAsync(e =>
                {
                    e.SetProperty(x => x.MqttKey, entity.MqttKey);
                    e.SetProperty(x => x.Verify, entity.Verify);
                }, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(DeviceApi entity, CancellationToken cancellationToken)
        {
            using PowerManagerContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            await DeleteAsync(context, entity, cancellationToken);
        }

        public async Task DeleteAsync(PowerManagerContext context, DeviceApi entity, CancellationToken cancellationToken)
        {
            await context.DeviceApi.Where(e => e.UserId.Equals(entity.UserId) && e.DeviceId.Equals(entity.DeviceId))
                .ExecuteDeleteAsync(cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
