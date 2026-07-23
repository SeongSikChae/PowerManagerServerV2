using Microsoft.AspNetCore.Mvc;
using PowerManager.Server.Context;
using PowerManager.Server.Context.Entity;
using PowerManager.Server.Context.Store;

namespace PowerManager.Server.Controllers
{
    [Route("rest/[controller]")]
    [ApiController]
    public class ElectricPowerController(PowerManagerContext context, IReadStore<PowerManagerContext, ElectricPower> store) : ControllerBase
    {
        [HttpGet("list")]
        public async Task<IEnumerable<ElectricPower>> GetListAsync(CancellationToken cancellationToken)
        {
            return await store.GetListAsync(context, cancellationToken);
        }
    }
}
