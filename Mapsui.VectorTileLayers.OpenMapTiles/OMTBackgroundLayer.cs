using BruTile;
using Mapsui.Extensions;
using Mapsui.Fetcher;
using Mapsui.Layers;
using Mapsui.Styles;
using Mapsui.Tiling.Extensions;
using Mapsui.VectorTileLayers.Core;
using Mapsui.VectorTileLayers.Core.Extensions;
using Mapsui.VectorTileLayers.Core.Styles;
using Mapsui.Widgets.ButtonWidgets;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Mapsui.VectorTileLayers.OpenMapTiles
{
    /// <summary>
    /// Layer for a tile oriented background
    /// </summary>
    /// <remarks>
    /// This Layer provides each time the same Feature which contains all tiles, that belong 
    /// to the given Viewport. All this tiles are later drawn by the style renderer.
    /// </remarks>
    public class OMTBackgroundLayer : ILayer
    {
        TileSchema _schema;
        readonly BackgroundTileFeature[] _features = { new BackgroundTileFeature() };

        public int Id => 0;

        public object Tag { get; set; }

        public double MinVisible => 24.ToResolution();

        public double MaxVisible => 0.ToResolution();

        public bool Enabled { get; set; } = true;

        public string Name { get; set; } = "Background";
        
        public MRect Extent => new Extent(-20037508.342789, -20037508.342789, 20037508.342789, 20037508.342789).ToMRect();

        public IStyle Style { get; set; }

        public double Opacity { get; set; }

        public bool Busy { get => false; set { } }

        public HyperlinkWidget Attribution => new HyperlinkWidget();

        public IReadOnlyList<double> Resolutions { get; }

        public bool IsMapInfoLayer { get => false; set { } }

        public Func<IEnumerable<IFeature>, IEnumerable<IFeature>> SortFeatures => (f) => (f);

        public string CustomLayerRendererName { get; set; }

        public event DataChangedEventHandler DataChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public OMTBackgroundLayer(OMTPaint paint)
        {
            Resolutions = new double[25];
            
            _schema = new TileSchema();
            _schema.Extent = Extent.ToExtent();
            _schema.OriginX = _schema.Extent.MinX;
            _schema.OriginY = _schema.Extent.MinY;
            _schema.Name = "Background";

            for (var i = 0; i <= 24; i++)
            {
                ((double[])Resolutions)[i] = i.ToResolution();
                _schema.Resolutions.Add(i, new Resolution(i, i.ToResolution(), 256, 256));
            }

            Style = new BackgroundTileStyle(paint);
        }

        public IEnumerable<IFeature> GetFeatures(MRect extent, double resolution)
        {
            _features[0].Tiles = _schema.GetTileInfos(extent.ToExtent(), (int)resolution.ToZoomLevel());

            return _features;
        }

        public void Dispose()
        {
        }

        public void DataHasChanged()
        {
        }

        public void RefreshData(FetchInfo fetchInfo)
        {
        }

        public bool UpdateAnimations()
        {
            return false;
        }
    }
}
