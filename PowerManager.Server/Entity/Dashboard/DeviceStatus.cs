namespace PowerManager.Server.Entity.Dashboard
{
    public sealed class DeviceStatus
    {
        public string? Version { get; set; }

        public string? DeviceIP { get; set; }

        public double Voltage { get; set; }

        public double ElectricCurrent { get; set; }

        public double Watt { get; set; }

        public double Temperature { get; set; }

        public SwitchStatus Switch { get; set; } = SwitchStatus.Unknown;
    }
}
