using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PowerManagerServer.Context
{
    [Table("Device")]
    public sealed class Device
    {
        [Key, StringLength(12)]
        public string ID { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string DeviceName { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string Model { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string Topic { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Password { get; set; }

        [Required]
        public ElectricPowerType PowerType { get; set; }

        public double? VoltCalibration { get; set; }

        [StringLength(50)]
        public string? ForwardConnector { get; set; }
    }

    [Table("UserDevice")]
    public sealed class UserDevice
    {
        [Key, StringLength(40)]
        public string Thumbprint { get; set; } = string.Empty;

        [Key, StringLength(12)]
        public string DeviceId { get; set; } = string.Empty;
    }
}
