using System.Collections.Generic;
using Mapsui.VectorTileLayer.Core.Interfaces;
using Mapsui.VectorTileLayer.Core.Enums;
using Mapsui.VectorTileLayer.Core.Filter;
using Mapsui.VectorTileLayer.Core.Primitives;
using Mapsui.VectorTileLayer.Core.Extensions;

namespace Mapsui.VectorTileLayer.MapboxGL
{
    public class MGLVectorTileStyle : IVectorTileStyle
    {
        public string Id { get; internal set; }

        public int MinZoom { get; internal set; }

        public int MaxZoom { get; internal set; }

        public string SourceLayer { get; internal set; }

        public StyleType Type { get; internal set; }

        public IFilter Filter { get; internal set; }

        public IEnumerable<IVectorPaint> Paints { get; internal set; } = new List<MGLPaint>();

        public IVectorSymbolStyler SymbolStyler { get; internal set; } = MGLSymbolStyler.Default;

        public bool IsVisible { get; internal set; } = true;

        public double MinVisible { get => MaxZoom.ToResolution(); set { MaxZoom = (int)value.ToZoomLevel(); } }

        public double MaxVisible { get => MinZoom.ToResolution(); set { MinZoom = (int)value.ToZoomLevel(); } }

        public bool Enabled { get => IsVisible; set { IsVisible = value; } }

        public float Opacity { get; set; } = 1f;

        public MGLVectorTileStyle()
        {
        }

        public void Update(EvaluationContext context)
        {
            // TODO: Update style
        }
    }
}
