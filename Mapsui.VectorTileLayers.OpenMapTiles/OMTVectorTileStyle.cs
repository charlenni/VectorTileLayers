using System.Collections.Generic;
using Mapsui.VectorTileLayers.Core.Interfaces;
using Mapsui.VectorTileLayers.Core.Enums;
using Mapsui.VectorTileLayers.Core.Filter;
using Mapsui.VectorTileLayers.Core.Primitives;
using Mapsui.VectorTileLayers.Core.Extensions;

namespace Mapsui.VectorTileLayers.OpenMapTiles
{
    public class OMTVectorTileStyle : IVectorTileStyle
    {
        public string Id { get; internal set; }

        public int MinZoom { get; internal set; }

        public int MaxZoom { get; internal set; }

        public string SourceLayer { get; internal set; }

        public StyleType Type { get; internal set; }

        public IFilter Filter { get; internal set; }

        public IEnumerable<IVectorPaint> Paints { get; internal set; } = new List<OMTPaint>();

        public IVectorSymbolStyler SymbolStyler { get; internal set; } = OMTSymbolStyler.Default;

        public bool IsVisible { get; internal set; } = true;

        public double MinVisible { get => MaxZoom.ToResolution(); set { MaxZoom = (int)value.ToZoomLevel(); } }

        public double MaxVisible { get => MinZoom.ToResolution(); set { MinZoom = (int)value.ToZoomLevel(); } }

        public bool Enabled { get => IsVisible; set { IsVisible = value; } }

        public float Opacity { get; set; } = 1f;

        public OMTVectorTileStyle()
        {
        }

        public void Update(EvaluationContext context)
        {
            // TODO: Update style
        }
    }
}
