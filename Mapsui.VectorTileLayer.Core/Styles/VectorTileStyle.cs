using Mapsui.VectorTileLayer.Core.Extensions;
using Mapsui.VectorTileLayer.Core.Interfaces;
using Mapsui.VectorTileLayer.Core.Primitives;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayer.Core.Styles
{
    public class VectorTileStyle : TileStyle
    {
        public VectorTileStyle(float minZoom, float maxZoom, IEnumerable<IVectorTileStyle> vectorStyles) : base(minZoom, maxZoom)
        {
            VectorTileStyles = new List<IVectorTileStyle>();

            foreach (var styleLayer in vectorStyles)
                ((List<IVectorTileStyle>)VectorTileStyles).Add(styleLayer);
        }

        public IEnumerable<IVectorTileStyle> VectorTileStyles { get; }

        public void UpdateStyles(IViewport viewport)
        {
            EvaluationContext context = new EvaluationContext((float)viewport.Resolution.ToZoomLevel());

            foreach (var vectorTileStyle in VectorTileStyles)
            {
                vectorTileStyle.Update(context);
            }
        }
    }
}
