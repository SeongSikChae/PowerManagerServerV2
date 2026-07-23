using PowerManager.Server.Context.Entity;

namespace PowerManager.Server.Entity.Dashboard
{
    public sealed class DeviceCard
    {
        public string ID { get; set; } = null!;

        public string DeviceName { get; set; } = null!;

        public string Model { get; set; } = null!;

        public ElectricPowerType PowerType { get; set; }

        public DeviceStatus? Status { get; set; }
    }
}
