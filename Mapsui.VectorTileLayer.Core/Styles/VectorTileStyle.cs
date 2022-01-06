using Mapsui.VectorTileLayer.Core.Extensions;
using Mapsui.VectorTileLayer.Core.Interfaces;
using Mapsui.VectorTileLayer.Core.Primitives;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayer.Core.Styles
{
    public class VectorTileStyle : TileStyle
    {
        public VectorTileStyle(float minZoom, float maxZoom, IEnumerable<IVectorStyleLayer> vectorStyles) : base(minZoom, maxZoom)
        {
            VectorStyles = new List<IVectorStyleLayer>();

            foreach (var styleLayer in vectorStyles)
                ((List<IVectorStyleLayer>)VectorStyles).Add(styleLayer);
        }

        public IEnumerable<IVectorStyleLayer> VectorStyles { get; }

        public void UpdateStyles(IViewport viewport)
        {
            EvaluationContext context = new EvaluationContext((float)viewport.Resolution.ToZoomLevel());

            foreach (var vectorStyleLayer in VectorStyles)
            {
                vectorStyleLayer.Update(context);
            }
        }
    }
}
