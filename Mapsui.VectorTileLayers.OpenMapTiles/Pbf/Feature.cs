namespace Mapsui.VectorTileLayers.OpenMapTiles.Pbf
{
    [ProtoBuf.ProtoContract(Name = @"feature")]
    public sealed class Feature : ProtoBuf.IExtensible
    {
        ulong _id;

        [ProtoBuf.ProtoMember(1, IsRequired = false, Name = @"id", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
        [System.ComponentModel.DefaultValue(default(ulong))]
        public ulong Id
        {
            get { return _id; }
            set { _id = value; }
        }
        readonly System.Collections.Generic.List<uint> _tags = new System.Collections.Generic.List<uint>();

        [ProtoBuf.ProtoMember(2, Name = @"tags", DataFormat = ProtoBuf.DataFormat.TwosComplement, Options = ProtoBuf.MemberSerializationOptions.Packed)]
        public System.Collections.Generic.List<uint> Tags
        {
            get { return _tags; }
        }

        GeomType _type = GeomType.Unknown;

        [ProtoBuf.ProtoMember(3, IsRequired = false, Name = @"type", DataFormat = ProtoBuf.DataFormat.TwosComplement)]
        [System.ComponentModel.DefaultValue(GeomType.Unknown)]
        public GeomType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        readonly System.Collections.Generic.List<uint> _geometry = new System.Collections.Generic.List<uint>();

        [ProtoBuf.ProtoMember(4, Name = @"geometry", DataFormat = ProtoBuf.DataFormat.TwosComplement, Options = ProtoBuf.MemberSerializationOptions.Packed)]
        public System.Collections.Generic.List<uint> Geometry
        {
            get { return _geometry; }
        }

        ProtoBuf.IExtension _extensionObject;

        ProtoBuf.IExtension ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return ProtoBuf.Extensible.GetExtensionObject(ref _extensionObject, createIfMissing); }
    }
}
