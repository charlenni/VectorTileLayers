using Mapsui.VectorTileLayers.Core.Extensions;
using Mapsui.VectorTileLayers.Core.Interfaces;
using Mapsui.VectorTileLayers.Core.Primitives;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayers.Core.Styles
{
    public class VectorTileStyle : TileStyle
    {
        public VectorTileStyle(float minZoom, float maxZoom, IEnumerable<IVectorStyle> styleLayers) : base(minZoom, maxZoom)
        {
            StyleLayers = new List<IVectorStyle>();

            foreach (var styleLayer in styleLayers)
                ((List<IVectorStyle>)StyleLayers).Add(styleLayer);
        }

        public IEnumerable<IVectorStyle> StyleLayers { get; }

        public void UpdateStyles(Viewport viewport)
        {
            EvaluationContext context = new EvaluationContext((float)viewport.Resolution.ToZoomLevel());

            foreach (var styleLayer in StyleLayers)
            {
                styleLayer.Update(context);
            }
        }
    }
}
