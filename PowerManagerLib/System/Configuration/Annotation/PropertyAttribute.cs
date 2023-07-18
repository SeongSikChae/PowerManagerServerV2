namespace System.Configuration.Annotation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class PropertyAttribute : Attribute
    {
        public PropertyAttribute(PropertyType propertyType, bool required)
        {
            Type = propertyType;
            Required = required;
        }

        public PropertyType Type { get; }

        public bool Required { get; }

        public string? Parent { get; set; }

        public string? DefaultValue { get; set; }

        public string? RequireMessage { get; set; }

        public enum PropertyType
        {
            BOOL,
            BYTE,
            SBYTE,
            SHORT,
            USHORT,
            INT,
            UINT,
            LONG,
            ULONG,
            DOUBLE,
            STRING,
            LIST
        }
    }
}
