using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PowerManager.Server.Context.Entity
{
    [Table("Device")]
    public sealed class Device
    {
        [Key, StringLength(12)]
        [JsonPropertyName("id")]
        public string ID { get; set; } = null!;

        [Required, StringLength(20)]
        public string DeviceName { get; set; } = null!;

        [Required, StringLength(20)]
        public string Model { get; set; } = null!;

        [Required, StringLength(20)]
        public string Topic { get; set; } = null!;

        [StringLength(50)]
        public string? Password { get; set; }

        [Required]
        public ElectricPowerType PowerType { get; set; }

        public double? VoltCalibration { get; set; }

        [StringLength(50)]
        public string? ForwardConnector { get; set; }
    }
}
