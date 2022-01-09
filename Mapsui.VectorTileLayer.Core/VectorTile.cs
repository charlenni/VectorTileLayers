using BruTile;
using Mapsui.Extensions;
using Mapsui.Styles;
using Mapsui.VectorTileLayer.Core.Enums;
using Mapsui.VectorTileLayer.Core.Interfaces;
using Mapsui.VectorTileLayer.Core.Primitives;
using Mapsui.VectorTileLayer.Core.Styles;
using System;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayer.Core
{
    public class VectorTile : IFeature, ITileDataSink
    {
        private readonly IEnumerable<IVectorStyleLayer> _styles;
        private readonly TileInfo _tileInfo;
        private readonly int _tileSize;
        private EvaluationContext _context;
        private Dictionary<IVectorStyleLayer, IBucket> _buckets = new Dictionary<IVectorStyleLayer, IBucket>();

        public VectorTile(TileInfo tileInfo, int tileSize, ref VectorTileStyle style, ref byte[] tileData, MRect extent)
        {
            _styles = style.VectorStyles;
            _tileSize = tileSize;
            _tileInfo = tileInfo;
            _context = new EvaluationContext(_tileInfo.Index.Level);
            Extent = _tileInfo.Extent.ToMRect();
        }

        public Dictionary<IVectorStyleLayer, IBucket> Buckets => _buckets;

        public TileInfo TileInfo => _tileInfo;

        public object this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ICollection<IStyle> Styles => null;

        public IEnumerable<string> Fields => throw new NotImplementedException();

        public MRect Extent { get; }

        public IDictionary<IStyle, object> RenderedGeometry => throw new NotImplementedException();

        public void CoordinateVisitor(Action<double, double, CoordinateSetter> visit)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        /// <summary>
        /// Function to fill the buckets with the right geometries
        /// </summary>
        /// <param name="element">VectorElement, which contains the geometry</param>
        public void Process(VectorElement element)
        {
            element.Scale(_tileSize / 4096.0f);

            // Now process this element and check, for which style layers it is ok
            foreach (var style in _styles)
            {
                // Is this style relevant or is it outside the zoom range
                if (!style.IsVisible || style.MinZoom > _tileInfo.Index.Level || style.MaxZoom < _tileInfo.Index.Level)
                    continue;

                // Is this style layer relevant for this feature?
                if (style.SourceLayer != element.Layer)
                    continue;

                // TODO: Remove, only for testing
                if (style.Type == StyleType.Symbol && style.SourceLayer == "poi")
                {
                    var name = style.SourceLayer;
                }

                // Fullfill element filter for this style layer
                if (!style.Filter.Evaluate(element))
                    continue;

                // Check for different types
                switch (style.Type)
                {
                    case StyleType.Symbol:
                        // Feature is a symbol
                        if (!_buckets.ContainsKey(style))
                            _buckets[style] = new SymbolBucket(style);
                        ((SymbolBucket)_buckets[style]).AddElement(element, _context);
                        break;
                    case StyleType.Line:
                        // Element is a line
                        if (element.IsLine && element.Count > 0)
                        {
                            if (!_buckets.ContainsKey(style))
                                _buckets[style] = new LineBucket();
                            ((LineBucket)_buckets[style]).AddElement(element);
                        }
                        else
                        {
                            // This are things like height of a building
                            // We don't use this up to now
                            //System.Diagnostics.Debug.WriteLine(element.Tags.ToString());
                        }
                        break;
                    case StyleType.Fill:
                        // Element is a fill
                        if (element.IsPolygon && element.Count > 0)
                        {
                            if (!_buckets.ContainsKey(style))
                                _buckets[style] = new FillBucket();
                            ((FillBucket)_buckets[style]).AddElement(element);
                        }
                        else
                        {
                            // This are things like height of a building
                            // We don't use this up to now
                            //System.Diagnostics.Debug.WriteLine(element.Tags.ToString());
                        }
                        break;
                    default:
                        // throw new Exception("Unknown style type");
                        break;
                }
            }
        }

        public void Completed(QueryResult result)
        {
            if (result == QueryResult.Succes)
            {
                List<IVectorStyleLayer> remove = new List<IVectorStyleLayer>();

                // Delete empty buckets
                foreach (var bucket in _buckets)
                {
                    if (bucket.Value is FillBucket && ((FillBucket)bucket.Value).Paths.Count == 0)
                    {
                        // Bucket is empty
                        remove.Add(bucket.Key);
                    }
                }

                if (remove.Count == 0)
                    return;

                foreach(var layer in remove)
                {
                    _buckets.Remove(layer);
                }
            }
        }
    }
}
