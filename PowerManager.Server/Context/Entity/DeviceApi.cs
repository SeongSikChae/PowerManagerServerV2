using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PowerManager.Server.Context.Entity
{
    [Table("DeviceApi")]
    public sealed class DeviceApi
    {
        [Key, StringLength(100)]
        public string UserId { get; set; } = null!;

        [Key, StringLength(12)]
        public string DeviceId { get; set; } = null!;

        [StringLength(100)]
        public string? Verify { get; set; }

        [StringLength(50)]
        public string? MqttKey { get; set; }
    }
}
