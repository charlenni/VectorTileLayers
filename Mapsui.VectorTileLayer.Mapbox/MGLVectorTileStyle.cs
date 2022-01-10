using System.Collections.Generic;
using Mapsui.VectorTileLayer.Core.Interfaces;
using Mapsui.VectorTileLayer.Core.Enums;
using Mapsui.VectorTileLayer.Core.Filter;
using Mapsui.VectorTileLayer.Core.Primitives;

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

        public MGLVectorTileStyle()
        {
        }

        public void Update(EvaluationContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
