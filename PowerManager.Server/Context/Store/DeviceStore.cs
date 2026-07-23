using Microsoft.EntityFrameworkCore;
using PowerManager.Server.Context.Entity;
using PowerManager.Server.Entity;
using System.Linq.Expressions;

namespace PowerManager.Server.Context.Store
{
    public sealed class DeviceStore(IDbContextFactory<PowerManagerContext> dbContextFactory) : IStore<PowerManagerContext, Device>
    {
        public async Task<bool> AnyAsync(Expression<Func<Device, bool>> expression, CancellationToken cancellationToken)
        {
            using PowerManagerContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            return await AnyAsync(context, expression, cancellationToken);
        }

        public async Task<bool> AnyAsync(PowerManagerContext context, Expression<Func<Device, bool>> expression, CancellationToken cancellationToken)
        {
            return await context.Device.AnyAsync(expression, cancellationToken);
        }

        public async ValueTask<Device?> FindAsync(object?[]? keyValues, CancellationToken cancellationToken)
        {
            using PowerManagerContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            return await FindAsync(context, keyValues, cancellationToken);
        }

        public async ValueTask<Device?> FindAsync(PowerManagerContext context, object?[]? keyValues, CancellationToken cancellationToken)
        {
            return await context.Device.FindAsync(keyValues, cancellationToken);
        }

        public async Task<List<Device>> GetListAsync(CancellationToken cancellationToken)
        {
            using PowerManagerContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            return await GetListAsync(context, cancellationToken);
        }

        public async Task<List<Device>> GetListAsync(PowerManagerContext context, CancellationToken cancellationToken)
        {
            return await context.Device.ToListAsync(cancellationToken);
        }

        public async Task<PagedResult<Device>> GetListAsync(SearchQuery query, CancellationToken cancellationToken)
        {
            using PowerManagerContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            return await GetListAsync(context, query, cancellationToken);
        }

        public async Task<PagedResult<Device>> GetListAsync(PowerManagerContext context, SearchQuery query, CancellationToken cancellationToken)
        {
            IQueryable<Device> filtered = context.Device
                .Where(entity => string.IsNullOrWhiteSpace(query.Query) || entity.ID.Contains(query.Query) || entity.DeviceName.Contains(query.Query));

            int totalCount = await filtered.CountAsync(cancellationToken);
            List<Device> items = await filtered
                .OrderBy(entity => entity.ID)
                .Skip((query.Pagination.CurrentPage - 1) * query.Pagination.ItemSize)
                .Take(query.Pagination.ItemSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Device>
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

        public async Task CreateAsync(Device device, CancellationToken cancellationToken)
        {
            using PowerManagerContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            await CreateAsync(context, device, cancellationToken);
        }

        public async Task CreateAsync(PowerManagerContext context, Device device, CancellationToken cancellationToken)
        {
            await context.Device.AddAsync(device, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Device device, CancellationToken cancellationToken)
        {
            using PowerManagerContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            await UpdateAsync(context, device, cancellationToken);
        }

        public async Task UpdateAsync(PowerManagerContext context, Device device, CancellationToken cancellationToken)
        {
            await context.Device
                .Where(entity => entity.ID.Equals(device.ID))
                .ExecuteUpdateAsync(entity =>
                    entity
                        .SetProperty(x => x.DeviceName, device.DeviceName)
                        .SetProperty(x => x.Model, device.Model)
                        .SetProperty(x => x.Topic, device.Topic)
                        .SetProperty(x => x.Password, device.Password)
                        .SetProperty(x => x.PowerType, device.PowerType)
                        .SetProperty(x => x.VoltCalibration, device.VoltCalibration)
                        .SetProperty(x => x.ForwardConnector, device.ForwardConnector),
                cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Device device, CancellationToken cancellationToken)
        {
            using PowerManagerContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            await DeleteAsync(context, device, cancellationToken);
        }

        public async Task DeleteAsync(PowerManagerContext context, Device device, CancellationToken cancellationToken)
        {
            await context.Device
                .Where(entity => entity.ID.Equals(device.ID))
                .ExecuteDeleteAsync(cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
