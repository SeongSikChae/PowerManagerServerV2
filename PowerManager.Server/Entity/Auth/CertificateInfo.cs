using System.Configuration.Annotation;

namespace PowerManager.Server.Entity.Auth
{
    public sealed class CertificateInfo
    {
        [Property(PropertyType.STRING, required: true)]
        public string CommonName { get; set; } = null!;

        [Property(PropertyType.STRING, required: true)]
        public string Email { get; set; } = null!;

        [Property(PropertyType.LONG, required: true)]
        public long? Before { get; set; } = null!;

        [Property(PropertyType.LONG, required: true)]
        public long? After { get; set; } = null!;

        [Property(PropertyType.STRING, required: true)]
        public string Thumbprint { get; set; } = null!;
    }
}
