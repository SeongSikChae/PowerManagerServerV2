using System.Configuration.Annotation;

namespace PowerManager.Server
{
    public sealed class Configuration
    {
        [Property(PropertyType.STRING, required: true)]
        public string ContentRootPath { get; set; } = null!;

        [Property(PropertyType.STRING, DefaultValue = "ClientApp")]
        public string WebRootPath { get; set; } = null!;

        [Property(PropertyType.USHORT, required: true)]
        public ushort? WebHttpPort { get; set; }

        [Property(PropertyType.USHORT, required: true)]
        public ushort? WebHttpsPort { get; set; }

        [Property(PropertyType.USHORT, DefaultValue = "1803")]
        public ushort? MqttPort { get; set; }

        [Property(PropertyType.USHORT, DefaultValue = "8883")]
        public ushort? MqttsPort { get; set; }

        [Property(PropertyType.STRING, required: true)]
        public string ServerCertificate { get; set; } = null!;

        [Property(PropertyType.STRING, required: true)]
        public string ServerCertificatePassword { get; set; } = null!;

        [Property(PropertyType.STRING, required: false)]
        public string? CertificateChain { get; set; }

        [Property(PropertyType.STRING, required: false)]
        public string? IncludeCipherSuites { get; set; }

        [Property(PropertyType.INT, DefaultValue = "100")]
        public int? MqttServerBacklog { get; set; }

        [Property(PropertyType.STRING, required: true)]
        public string DbPath { get; set; } = null!;
    }
}
