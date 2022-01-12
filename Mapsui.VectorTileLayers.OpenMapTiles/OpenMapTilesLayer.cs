using Mapsui.Layers;
using Mapsui.VectorTileLayers.Core.Enums;
using Mapsui.VectorTileLayers.Core.Styles;
using System;
using System.IO;

namespace Mapsui.VectorTileLayers.OpenMapTiles
{
    public class OpenMapTilesLayer : LayerCollection
    {
        public OpenMapTilesLayer(Stream styleFile, Func<LocalContentType, string, Stream> getLocalContent = null)
        {
            if (styleFile == null)
                throw new ArgumentNullException($"{nameof(styleFile)} should not be null");

            OMTStyleFileLoader.GetLocalContent = getLocalContent;

            // Get Mapbox GL Style File
            var mglStyleFile = OMTStyleFileLoader.Load(styleFile);

            if (mglStyleFile == null)
                return;

            // Ok, we have a valid style file, so get the tile layers, contained in style file
            foreach (var tileLayer in mglStyleFile.TileLayers)
            {
                switch (tileLayer.Style)
                {
                    case BackgroundTileStyle backgroundTileStyle:
                        break;
                    case RasterTileStyle rasterTileStyle:
                        break;
                    case VectorTileStyle vectorTileStyle:
                        break;
                }

                //tileLayer.MinVisible = tileLayer.MaxVisible < 24.ToResolution() ? 24.ToResolution() : tileLayer.MinVisible;
                //tileLayer.MaxVisible = tileLayer.MaxVisible > 0.ToResolution() ? 0.ToResolution() : tileLayer.MaxVisible;
                
                Add(tileLayer);
            }

        }
    }
}
