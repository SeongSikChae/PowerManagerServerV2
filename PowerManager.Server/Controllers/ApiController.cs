using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PowerManager.Server.Context;
using PowerManager.Server.Context.Entity;
using PowerManager.Server.Context.Store;
using PowerManager.Server.Entity.Api;
using System.Text.RegularExpressions;

namespace PowerManager.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApiController(PowerManagerContext context, IStore<PowerManagerContext, DeviceApi> deviceApiStore) : ControllerBase
    {
        private readonly Regex UserIdPattern = new Regex(@"DAWONDNS-(?<Model>\S+)-(?<DeviceId>\S+)", RegexOptions.NonBacktracking);

        [HttpGet("v1/device/connect/{device}")]
        [AllowAnonymous]
        public async Task<ConnectResponse> ConnectAsync(string device, CancellationToken cancellationToken)
        {
            Match match = UserIdPattern.Match(device);
            if (!match.Success)
                throw new ArgumentException($"Invalid UserId: {device}");
            string deviceId = match.Groups["DeviceId"].Value;
            if (await deviceApiStore.AnyAsync(context, v => v.DeviceId.Equals(deviceId), cancellationToken))
                return new ConnectResponse();
            throw new Exception("Device Auth Not Found");
        }
    }
}
