using System.Configuration.Annotation;

namespace PowerManagerServer
{
    public sealed class Configuration
    {
        [Property(PropertyAttribute.PropertyType.USHORT, required: false, DefaultValue = "80")]
        public ushort WebHttpPort { get; set; }

        [Property(PropertyAttribute.PropertyType.USHORT, required: false, DefaultValue = "443")]
        public ushort WebHttpsPort { get; set; }

        [Property(PropertyAttribute.PropertyType.USHORT, required: false, DefaultValue = "1803")]
        public ushort MqttPort { get; set; }

        [Property(PropertyAttribute.PropertyType.USHORT, required: false, DefaultValue = "8883")]
        public ushort MqttsPort { get; set; }

        [Property(PropertyAttribute.PropertyType.USHORT, required: false, DefaultValue = "18443")]
        public ushort ApiPort { get; set; }

        [Property(PropertyAttribute.PropertyType.USHORT, required: false, DefaultValue = "18446")]
        public ushort HttpApiPort { get; set; }

        [Property(PropertyAttribute.PropertyType.STRING, required: true)]
        public string ServerCertificate { get; set; } = string.Empty;

        [Property(PropertyAttribute.PropertyType.STRING, required: true)]
        public string ServerCertificatePassword { get; set; } = string.Empty;

        [Property(PropertyAttribute.PropertyType.INT, required: false, DefaultValue = "100")]
        public int MqttServerBacklog { get; set; }

        [Property(PropertyAttribute.PropertyType.BOOL, required: false)]
        public bool HTTP2 { get; set; }

        [Property(PropertyAttribute.PropertyType.LIST, required: false)]
        public List<string> IncludeCipherSuites { get; set; } = new List<string>();

        [Property(PropertyAttribute.PropertyType.LIST, required: false)]
        public List<string> IncludeCipherSuitesForApi { get; set; } = new List<string>();

        [Property(PropertyAttribute.PropertyType.STRING, required: true)]
        public string DbPath { get; set; } = string.Empty;

        [Property(PropertyAttribute.PropertyType.INT, false, DefaultValue = "30000")]
        public int MqttKeepAlliveInterval { get; set; }

        [Property(PropertyAttribute.PropertyType.STRING, required: false)]
        public string? TelegramToken { get; set; }

        [Property(PropertyAttribute.PropertyType.UINT, required: false, DefaultValue = "3")]
        public uint CollectionInterval { get; set; }

        [Property(PropertyAttribute.PropertyType.STRING, required: false, DefaultValue = "iot-server")]
        public string InternalMqttUserName { get; set; } = "iot-server";

        [Property(PropertyAttribute.PropertyType.STRING, required: false, DefaultValue = "1234")]
        public string InternalMqttUserPassword { get; set; } = "1234";

        [Property(PropertyAttribute.PropertyType.USHORT, required: false, DefaultValue = "60")]
        public ushort ForwardSuppressTime { get; set; }
    }
}
