namespace Mapsui.VectorTileLayer.MapboxGL.Pbf
{
    [ProtoBuf.ProtoContract(Name = @"value")]
    public sealed class Value : ProtoBuf.IExtensible
    {
        string _stringValue = "";

        public bool HasStringValue { get; set; }
        public bool HasFloatValue { get; set; }
        public bool HasDoubleValue { get; set; }
        public bool HasIntValue { get; set; }
        public bool HasUIntValue { get; set; }
        public bool HasSIntValue { get; set; }
        public bool HasBoolValue { get; set; }

        public bool ShouldSerializeStringValue() => HasStringValue;
        public bool ShouldSerializeFloatValue() => HasFloatValue;
        public bool ShouldSerializeDoubleValue() => HasDoubleValue;
        public bool ShouldSerializeIntValue() => HasIntValue;
        public bool ShouldSerializeUIntValue() => HasUIntValue;
        public bool ShouldSerializeSIntValue() => HasSIntValue;
        public bool ShouldSerializeBoolValue() => HasBoolValue;

        [ProtoBuf.ProtoMember(1, IsRequired = false, Name = @"string_value", DataFormat = ProtoBuf.DataFormat.Default)]
        [System.ComponentModel.DefaultValue("")]
        public string StringValue
        {
            get { return _stringValue; }
            set
            {
                HasStringValue = true;
                _stringValue = value;
            }
        }

        float _floatValue;
        [ProtoBuf.ProtoMember(2, IsRequired = false, Name = @"float_value", DataFormat = ProtoBuf.DataFormat.FixedSize)]
        [System.ComponentModel.DefaultValue(default(float))]
        public float FloatValue
        {
            get
            {
                return _floatValue;
            }
            set
            {
                _floatValue = value;
                HasFloatValue = true;

            }
        }
        double _doubleValue;
        [ProtoBuf.ProtoMember(3, IsRequired = false, Name = @"double_value", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
        [System.ComponentModel.DefaultValue(default(double))]
        public double DoubleValue
        {
            get { return _doubleValue; }
            set
            {
                _doubleValue = value;
                HasDoubleValue = true;
            }
        }
        long _intValue;
        [ProtoBuf.ProtoMember(4, IsRequired = false, Name = @"int_value", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
        [System.ComponentModel.DefaultValue(default(long))]
        public long IntValue
        {
            get { return _intValue; }
            set
            {
                _intValue = value;
                HasIntValue = true;
            }
        }
        ulong _uintValue;
        [ProtoBuf.ProtoMember(5, IsRequired = false, Name = @"uint_value", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
        [System.ComponentModel.DefaultValue(default(ulong))]
        public ulong UintValue
        {
            get { return _uintValue; }
            set
            {
                _uintValue = value;
                HasUIntValue = true;
            }
        }
        long _sintValue;
        [ProtoBuf.ProtoMember(6, IsRequired = false, Name = @"sint_value", DataFormat = ProtoBuf.DataFormat.ZigZag)]
        [System.ComponentModel.DefaultValue(default(long))]
        public long SintValue
        {
            get { return _sintValue; }
            set
            {
                _sintValue = value;
                HasSIntValue = true;
            }
        }
        bool _boolValue;
        [ProtoBuf.ProtoMember(7, IsRequired = false, Name = @"bool_value", DataFormat = ProtoBuf.DataFormat.Default)]
        [System.ComponentModel.DefaultValue(default(bool))]
        public bool BoolValue
        {
            get { return _boolValue; }
            set
            {
                _boolValue = value;
                HasBoolValue = true;
            }
        }
        ProtoBuf.IExtension _extensionObject;
        ProtoBuf.IExtension ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return ProtoBuf.Extensible.GetExtensionObject(ref _extensionObject, createIfMissing); }
    }
}
