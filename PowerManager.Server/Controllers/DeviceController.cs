using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PowerManager.Server.Context;
using PowerManager.Server.Context.Entity;
using PowerManager.Server.Context.Exceptions;
using PowerManager.Server.Context.Store;
using PowerManager.Server.Entity;

namespace PowerManager.Server.Controllers
{
    [Route("rest/[controller]")]
    [ApiController]
    public class DeviceController(PowerManagerContext context, IStore<PowerManagerContext, Device> deviceStore) : ControllerBase
    {
        [HttpPost("list")]
        [Authorize]
        public async Task<PagedResult<Device>> GetListAsync([FromBody] SearchQuery query, CancellationToken cancellationToken)
        {
            return await deviceStore.GetListAsync(context, query, cancellationToken);
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<RestResult> CreateAsync([FromBody] Device device, CancellationToken cancellationToken)
        {
            if (await deviceStore.AnyAsync(entity => entity.ID.Equals(device.ID), cancellationToken))
                throw new DuplicateKeyException(device.ID, null);
            await deviceStore.CreateAsync(context, device, cancellationToken);
            return RestResult.Success();
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<RestResult> UpdateAsync([FromBody] Device device, CancellationToken cancellationToken)
        {
            if (!await deviceStore.AnyAsync(entity => entity.ID.Equals(device.ID), cancellationToken))
                throw new KeyNotFoundException();
            await deviceStore.UpdateAsync(context, device, cancellationToken);
            return RestResult.Success();
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<RestResult> DeleteAsync(string id, CancellationToken cancellationToken)
        {
            Device? other = await deviceStore.FindAsync([id], cancellationToken) ?? throw new KeyNotFoundException();
            await deviceStore.DeleteAsync(context, other, cancellationToken);
            return RestResult.Success();
        }
    }
}
