using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Mapsui.VectorTileLayers.Core.Filter;

namespace Mapsui.VectorTileLayers.OpenMapTiles.Json
{
    public class JsonStyleLayer
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("paint")]
        public JsonPaint Paint { get; set; }

        [JsonProperty("interactive")]
        public bool Interactive { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        private string _sourceLayer;

        [JsonProperty("source-layer")]
        public string SourceLayer
        {
            get => _sourceLayer;
            set
            {
                _sourceLayer = value;
                SourceLayerHash = value.GetHashCode();
            }
        }

        [JsonProperty("filter")]
        public JArray NativeFilter { get; set; }

        public IFilter Filter { get; set; }

        //[JsonProperty("metadata")]
        //public Metadata { get; set; }

        [JsonProperty("layout")]
        public JsonLayout Layout { get; set; }

        [JsonProperty("ref")]
        public string Ref { get; set; }

        private float? _maxZoom;

        [JsonProperty("maxzoom")]
        public float? MaxZoom
        {
            get => _maxZoom;
            set
            {
                _maxZoom = value;
            }
        }

        private float? _minZoom;

        [JsonProperty("minzoom")]
        public float? MinZoom
        {
            get => _minZoom;
            set
            {
                _minZoom = value;
            }
        }

        public int SourceLayerHash { get; set; }

        public int ZIndex { get; set; }

        public override string ToString()
        {
            return Id + " " + Type;
        }
    }
}
