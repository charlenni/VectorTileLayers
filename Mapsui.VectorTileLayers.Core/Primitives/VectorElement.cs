using BruTile;
using Mapsui.VectorTileLayers.Core.Enums;
using Mapsui.VectorTileLayers.Core.Extensions;
using Mapsui.VectorTileLayers.Core.Interfaces;
using Mapsui.VectorTileLayers.Core.Utilities;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayers.Core.Primitives
{
    /// <summary>
    /// A VectorElement holds informations such as points and tags
    /// </summary>
    public class VectorElement : IVectorElement
    {
        readonly List<MPoint> _points = new List<MPoint>(512);
        readonly List<int> _index = new List<int>(64);
        readonly TileClipper _tileClipper;
        readonly float _tileSizeOfData;

        public VectorElement(TileClipper clipper, TileIndex tileIndex, float tileSizeOfData)
        {
            _tileClipper = clipper;
            _index.Add(0);
            _tileSizeOfData = tileSizeOfData;

            TileIndex = tileIndex;
        }

        public VectorElement(TileClipper clipper, TileIndex index, float TileSizeOfData, string layer, string id) : this(clipper, index, TileSizeOfData)
        {
            Layer = layer;
            Id = id;
        }

        public string Layer { get; set; }

        public string Id { get; set; }

        public TileIndex TileIndex { get; }

        public GeometryType Type { get; private set; }

        public TagsCollection Tags { get; } = new TagsCollection();

        public List<MPoint> Points { get => IsPoint ? new List<MPoint>(_points) : new List<MPoint>(); }

        public bool IsPoint { get => Type == GeometryType.Point; }

        public bool IsLine { get => Type == GeometryType.LineString; }

        public bool IsPolygon { get => Type == GeometryType.Polygon; }

        public int Count { get => IsPoint ? _points.Count : _index.Count; }

        public void Add(MPoint point)
        {
            // TODO: Check for correct values
            if (IsPoint && (point.X < 0 || point.X > _tileSizeOfData || point.Y < 0 || point.Y > _tileSizeOfData))
                return;

            _points.Add(point);
            _index[_index.Count - 1]++;
        }

        public void Add(float x, float y)
        {
            Add(new MPoint(x, y));
        }

        public void Add(IEnumerable<MPoint> points)
        {
            foreach (var point in points)
                Add(point);
        }

        public MPoint Get(int index)
        {
            if (index >= 0 && index < Count)
                return _points[index];

            return new MPoint(0, 0);
        }

        public void Clear()
        {
            _index.Clear();
            _index.Add(0);
            _points.Clear();
            Tags.Clear();
            Type = GeometryType.Unknown;
        }

        public void AddToPath(SKPath path)
        {
            int start = 0;

            for (int i = 0; i < _index.Count; i++)
            {
                if (_index[i] > 0)
                {
                    if (IsPolygon)
                        path.AddPoly(_tileClipper.ReducePolygonPointsToClipRect(_points.GetRange(start, _index[i])).ToArray().ToSKPoints(), true);
                    else if (IsLine)
                    {
                        var lines = _tileClipper.ReduceLinePointsToClipRect(_points.GetRange(start, _index[i]));
                        foreach (var line in lines)
                            path.AddPoly(line.ToArray().ToSKPoints(), false);
                    }
                }

                start += _index[i];
            }
        }

        public SKPath CreatePath()
        {
            SKPath path = new SKPath();

            AddToPath(path);

            if (path.PointCount > 0)
                return path;

            return null;
        }

        public void Scale(float factor)
        {
            if (factor == 1)
                return;

            for (int i = 0; i < _points.Count; i++)
            {
                _points[i] = new MPoint(_points[i].X * factor, _points[i].Y * factor);
            }
        }

        public void StartPoint()
        {
            SetOrCheckMode(GeometryType.Point);
        }

        public void StartLine()
        {
            SetOrCheckMode(GeometryType.LineString);

            if (_index[_index.Count - 1] > 0)
                _index.Add(0);
        }

        public void StartPolygon()
        {
            SetOrCheckMode(GeometryType.Polygon);

            if (_index[_index.Count - 1] > 0)
                _index.Add(0);
        }

        public void StartHole()
        {
            if (Type != GeometryType.Polygon)
                throw new ArgumentException($"Wrong type mode. Expected {GeometryType.Polygon}, but get {Type}.");

            if (_index[_index.Count - 1] > 0)
                _index.Add(0);
        }

        void SetOrCheckMode(GeometryType type)
        {
            if (Type == type)
                return;

            if (Type != GeometryType.Unknown)
                throw new ArgumentException($"Wrong type mode. Expected {Type}, but get {type}.");

            Type = type;
        }
    }
}
