using Newtonsoft.Json;
using SkiaSharp;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayer.MapboxGL.Json
{
    /// <summary>
    /// Class holding StoppedColor data in Json format
    /// </summary>
    public class JsonStoppedColor
    {
        [JsonProperty("base")]
        public float Base { get; set; } = 1f;

        [JsonProperty("stops")]
        public IList<KeyValuePair<float, SKColor>> Stops { get; set; }
    }
}
