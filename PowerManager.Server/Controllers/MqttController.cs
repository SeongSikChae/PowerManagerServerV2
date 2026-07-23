using CommandLine;
using Microsoft.EntityFrameworkCore;
using MQTTnet.Server;
using PowerManager.Server.Context;
using PowerManager.Server.Context.Entity;
using PowerManager.Server.Context.Store;
using PowerManager.Server.Entity.Dashboard;
using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;

namespace PowerManager.Server.Controllers
{
    public class MqttController(MqttServer server, IDbContextFactory<PowerManagerContext> dbContextFactory, IStore<PowerManagerContext, Device> deviceStore)
    {
        private const string DEVICE_ID_KEY = "DeviceId";

        private readonly Regex ClientIdPattern = new Regex(@"DAWONDNS-(?<DeviceId>\S+)", RegexOptions.NonBacktracking);
        private readonly ConcurrentDictionary<string, DeviceCard> CardDic = new ConcurrentDictionary<string, DeviceCard>();

        public async Task ValidatingConnectionAsync(ValidatingConnectionEventArgs context)
        {
            if (context.ClientId is null)
            {
                context.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.ClientIdentifierNotValid;
                context.ReasonString = "ClientId Not Found";
                return;
            }

            Match match = ClientIdPattern.Match(context.ClientId);
            if (!match.Success)
            {
                context.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.ClientIdentifierNotValid;
                context.ReasonString = $"Invalid ClientId: '{context.ClientId}'";
                return;
            }

            string deviceId = match.Groups["DeviceId"].Value;

            using PowerManagerContext dbContext = await dbContextFactory.CreateDbContextAsync(context.CancellationToken);
            Device? device = await deviceStore.FindAsync([deviceId], context.CancellationToken);
            if (device is null)
            {
                context.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.ClientIdentifierNotValid;
                context.ReasonString = $"Invalid ClientId: '{context.ClientId}'";
                return;
            }

            string expectedUserName = $"DAWONDNS-{device.Model}-{device.ID}";
            if (!expectedUserName.Equals(context.UserName))
            {
                context.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.BadUserNameOrPassword;
                context.ReasonString = "Invalid UserNameOrPassword";
                return;
            }

            if (!Equals(device.Password, context.Password))
            {
                context.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.BadUserNameOrPassword;
                context.ReasonString = "Invalid UserNameOrPassword";
                return;
            }

            context.SessionItems[DEVICE_ID_KEY] = deviceId;
            context.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.Success;
        }

        public async Task ClientConnectedAsync(ClientConnectedEventArgs context)
        {
            using PowerManagerContext dbContext = await dbContextFactory.CreateDbContextAsync();
            Device? device = await deviceStore.FindAsync([context.SessionItems[DEVICE_ID_KEY]], CancellationToken.None);
            if (device is null)
            {
                await server.DisconnectClientAsync(context.ClientId, MQTTnet.Protocol.MqttDisconnectReasonCode.NotAuthorized);
                return;
            }

            DeviceCard card = new DeviceCard
            {
                ID = device.ID,
                DeviceName = device.DeviceName,
                Model = device.Model,
                PowerType = device.PowerType
            };
            CardDic.AddOrUpdate(device.ID, card, (k, v) => card);
        }

        public Task InterceptingPublishAsync(InterceptingPublishEventArgs args)
        {
            return Task.CompletedTask;
        }

        public Task ClientDisconnectedAsync(ClientDisconnectedEventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}
