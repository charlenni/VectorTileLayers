namespace Mapsui.VectorTileLayers.OpenMapTiles.Pbf
{
    [ProtoBuf.ProtoContract(Name = @"tile")]
    public sealed class Tile : ProtoBuf.IExtensible
    {
        readonly System.Collections.Generic.List<Layer> _layers = new System.Collections.Generic.List<Layer>();

        [ProtoBuf.ProtoMember(3, Name = @"layers", DataFormat = ProtoBuf.DataFormat.Default)]
        public System.Collections.Generic.List<Layer> Layers
        {
            get { return _layers; }
        }

        ProtoBuf.IExtension _extensionObject;

        ProtoBuf.IExtension ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return ProtoBuf.Extensible.GetExtensionObject(ref _extensionObject, createIfMissing); }
    }
}
