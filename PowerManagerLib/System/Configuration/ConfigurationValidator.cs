namespace System.Configuration
{
    using Annotation;
    using Reflection;

    public static class ConfigurationValidator
    {
        public static void Validate<T>(T? config)
        {
            ArgumentNullException.ThrowIfNull(config);

            Type type = config.GetType();
            IEnumerable<PropertyInfo> properties = type.GetTypeInfo().DeclaredProperties;

            Dictionary<string, IProperty> d = new();
            foreach (PropertyInfo propertyInfo in properties)
            {
                PropertyAttribute? propertyAttribute = propertyInfo.GetCustomAttribute<PropertyAttribute>();
                if (propertyAttribute != null)
                    d.Add(propertyInfo.Name, IProperty.Of<T>(config, propertyInfo, propertyAttribute));
            }
            foreach (PropertyInfo propertyInfo in properties)
            {
                PropertyAttribute? propertyAttribute = propertyInfo.GetCustomAttribute<PropertyAttribute>();
                if (propertyAttribute != null)
                {
                    IProperty property = d[propertyInfo.Name];
                    IProperty? parent = null;
                    if (propertyAttribute.Parent != null && !string.IsNullOrWhiteSpace(propertyAttribute.Parent))
                    {
                        if (!d.ContainsKey(propertyAttribute.Parent))
                            throw new Exception($"parent config property '{propertyAttribute.Parent}' not found");
                        parent = d[propertyAttribute.Parent];
                    }
                    if (propertyAttribute.Required && !property.IsValuePresent && (parent is null || (parent != null && parent.IsValuePresent)))
                    {
                        if (propertyAttribute.RequireMessage != null && !string.IsNullOrWhiteSpace(propertyAttribute.RequireMessage))
                            throw new Exception(propertyAttribute.RequireMessage);
                        else
                            throw new Exception($"config field '{propertyInfo.Name}' must be provided");
                    }
                    if (!propertyAttribute.Required && propertyAttribute.DefaultValue != null && !string.IsNullOrWhiteSpace(propertyAttribute.DefaultValue) && !property.IsValuePresent && (parent == null || (parent != null && parent.IsValuePresent)))
                    {
                        switch (propertyAttribute.Type)
                        {
                            case PropertyAttribute.PropertyType.BOOL:
                                propertyInfo.SetValue(config, bool.Parse(propertyAttribute.DefaultValue));
                                break;
                            case PropertyAttribute.PropertyType.BYTE:
                                propertyInfo.SetValue(config, byte.Parse(propertyAttribute.DefaultValue));
                                break;
                            case PropertyAttribute.PropertyType.SBYTE:
                                propertyInfo.SetValue(config, sbyte.Parse(propertyAttribute.DefaultValue));
                                break;
                            case PropertyAttribute.PropertyType.SHORT:
                                propertyInfo.SetValue(config, short.Parse(propertyAttribute.DefaultValue));
                                break;
                            case PropertyAttribute.PropertyType.USHORT:
                                propertyInfo.SetValue(config, ushort.Parse(propertyAttribute.DefaultValue));
                                break;
                            case PropertyAttribute.PropertyType.INT:
                                propertyInfo.SetValue(config, int.Parse(propertyAttribute.DefaultValue));
                                break;
                            case PropertyAttribute.PropertyType.UINT:
                                propertyInfo.SetValue(config, uint.Parse(propertyAttribute.DefaultValue));
                                break;
                            case PropertyAttribute.PropertyType.LONG:
                                propertyInfo.SetValue(config, long.Parse(propertyAttribute.DefaultValue));
                                break;
                            case PropertyAttribute.PropertyType.ULONG:
                                propertyInfo.SetValue(config, ulong.Parse(propertyAttribute.DefaultValue));
                                break;
                            case PropertyAttribute.PropertyType.DOUBLE:
                                propertyInfo.SetValue(config, double.Parse(propertyAttribute.DefaultValue));
                                break;
                            case PropertyAttribute.PropertyType.STRING:
                                propertyInfo.SetValue(config, propertyAttribute.DefaultValue);
                                break;
                        }
                    }
                }
            }
        }
    }

    internal interface IProperty
    {
        bool IsValuePresent { get; }

        internal sealed class BoolProperty : IProperty
        {
            public BoolProperty(bool? v)
            {
                this.v = v;
            }

            private readonly bool? v;

            public bool IsValuePresent => v.HasValue;
        }

        internal sealed class ByteProperty : IProperty
        {
            public ByteProperty(byte? v)
            {
                this.v = v;
            }

            private readonly byte? v;

            public bool IsValuePresent => v.HasValue && v.Value != 0;
        }

        internal sealed class SByteProperty : IProperty
        {
            public SByteProperty(sbyte? v)
            {
                this.v = v;
            }

            private readonly sbyte? v;

            public bool IsValuePresent => v.HasValue && v.Value != 0;
        }

        internal sealed class Int16Property : IProperty
        {
            public Int16Property(short? v)
            {
                this.v = v;
            }

            private readonly short? v;

            public bool IsValuePresent => v.HasValue && v.Value != 0;
        }

        internal sealed class UInt16Property : IProperty
        {
            public UInt16Property(ushort? v)
            {
                this.v = v;
            }

            private readonly ushort? v;

            public bool IsValuePresent => v.HasValue && v.Value != 0;
        }

        internal sealed class Int32Property : IProperty
        {
            public Int32Property(int? v)
            {
                this.v = v;
            }

            private readonly int? v;

            public bool IsValuePresent => v.HasValue && v.Value != 0;
        }

        internal sealed class UInt32Property : IProperty
        {
            public UInt32Property(uint? v)
            {
                this.v = v;
            }

            private readonly uint? v;

            public bool IsValuePresent => v.HasValue && v.Value != 0;
        }

        internal sealed class Int64Property : IProperty
        {
            public Int64Property(long? v)
            {
                this.v = v;
            }

            private readonly long? v;

            public bool IsValuePresent => v.HasValue && v.Value != 0;
        }

        internal sealed class UInt64Property : IProperty
        {
            public UInt64Property(ulong? v)
            {
                this.v = v;
            }

            private readonly ulong? v;

            public bool IsValuePresent => v.HasValue && v.Value != 0;
        }

        internal sealed class DoubleProperty : IProperty
        {
            public DoubleProperty(double? v)
            {
                this.v = v;
            }

            private readonly double? v;

            public bool IsValuePresent => v.HasValue && v.Value != 0;
        }

        internal sealed class StringProperty : IProperty
        {
            public StringProperty(string v)
            {
                this.v = v;
            }

            private readonly string v;

            public bool IsValuePresent => v != null;
        }

        internal sealed class ListProperty : IProperty
        {
            public ListProperty(dynamic v)
            {
                this.v = v;
            }

            private readonly dynamic v;

            public bool IsValuePresent => v != null;
        }

        public static IProperty Of<SourceType>(SourceType o, PropertyInfo p, PropertyAttribute attribute)
        {
            switch (attribute.Type)
            {
                case PropertyAttribute.PropertyType.BOOL:
                    return new BoolProperty((bool?)p.GetValue(o));
                case PropertyAttribute.PropertyType.BYTE:
                    return new ByteProperty((byte?)p.GetValue(o));
                case PropertyAttribute.PropertyType.SBYTE:
                    return new SByteProperty((sbyte?)p.GetValue(o));
                case PropertyAttribute.PropertyType.SHORT:
                    return new Int16Property((short?)p.GetValue(o));
                case PropertyAttribute.PropertyType.USHORT:
                    return new UInt16Property((ushort?)p.GetValue(o));
                case PropertyAttribute.PropertyType.INT:
                    return new Int32Property((int?)p.GetValue(o));
                case PropertyAttribute.PropertyType.UINT:
                    return new UInt32Property((uint?)p.GetValue(o));
                case PropertyAttribute.PropertyType.LONG:
                    return new Int64Property((long?)p.GetValue(o));
                case PropertyAttribute.PropertyType.ULONG:
                    return new UInt64Property((ulong?)p.GetValue(o));
                case PropertyAttribute.PropertyType.DOUBLE:
                    return new DoubleProperty((double?)p.GetValue(o));
                case PropertyAttribute.PropertyType.STRING:
                    return new StringProperty(p.GetValue(o) as string ?? string.Empty);
                case PropertyAttribute.PropertyType.LIST:
                    return new ListProperty(p.GetValue(o) as dynamic);
                default:
                    throw new Exception($"unknown type '{attribute.Type}'");
            }
        }
    }
}
