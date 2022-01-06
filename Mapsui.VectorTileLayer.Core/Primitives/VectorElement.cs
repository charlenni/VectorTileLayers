using BruTile;
using Mapsui.VectorTileLayer.Core.Enums;
using Mapsui.VectorTileLayer.Core.Extensions;
using Mapsui.VectorTileLayer.Core.Interfaces;
using Mapsui.VectorTileLayer.Core.Utilities;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayer.Core.Primitives
{
    /// <summary>
    /// A VectorElement holds informations such as points and tags
    /// </summary>
    public class VectorElement : IVectorElement
    {
        List<MPoint> points = new List<MPoint>(512);
        List<int> index = new List<int>(64);
        TileClipper tileClipper;

        public VectorElement(TileClipper clipper, TileIndex tileIndex)
        {
            tileClipper = clipper;
            index.Add(0);

            TileIndex = tileIndex;
        }

        public VectorElement(TileClipper clipper, TileIndex index, string layer, string id) : this(clipper, index)
        {
            Layer = layer;
            Id = id;
        }

        public string Layer { get; set; }

        public string Id { get; set; }

        public TileIndex TileIndex { get; }

        public GeometryType Type { get; private set; }

        public TagsCollection Tags { get; } = new TagsCollection();

        public List<MPoint> Points { get => IsPoint ? new List<MPoint>(points) : new List<MPoint>(); }

        public bool IsPoint { get => Type == GeometryType.Point; }

        public bool IsLine { get => Type == GeometryType.LineString; }

        public bool IsPolygon { get => Type == GeometryType.Polygon; }

        public int Count { get => IsPoint ? points.Count : index.Count; }

        public void Add(MPoint point)
        {
            // TODO: Check for correct values
            if (IsPoint && (point.X < 0 || point.X > 4096 || point.Y < 0 || point.Y > 4096))
                return;

            points.Add(point);
            index[index.Count - 1]++;
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
                return points[index];

            return new MPoint(0, 0);
        }

        public void Clear()
        {
            index.Clear();
            index.Add(0);
            points.Clear();
            Tags.Clear();
            Type = GeometryType.Unknown;
        }

        public void AddToPath(SKPath path)
        {
            int start = 0;

            for (int i = 0; i < index.Count; i++)
            {
                if (index[i] > 0)
                {
                    if (IsPolygon)
                        path.AddPoly(tileClipper.ReducePolygonPointsToClipRect(points.GetRange(start, index[i])).ToArray().ToSKPoints(), true);
                    else if (IsLine)
                    {
                        var lines = tileClipper.ReduceLinePointsToClipRect(points.GetRange(start, index[i]));
                        foreach (var line in lines)
                            path.AddPoly(line.ToArray().ToSKPoints(), false);
                    }
                }

                start += index[i];
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

            for (int i = 0; i < points.Count; i++)
            {
                points[i] = new MPoint(points[i].X * factor, points[i].Y * factor);
            }
        }

        public void StartPoint()
        {
            SetOrCheckMode(GeometryType.Point);
        }

        public void StartLine()
        {
            SetOrCheckMode(GeometryType.LineString);

            if (index[index.Count - 1] > 0)
                index.Add(0);
        }

        public void StartPolygon()
        {
            SetOrCheckMode(GeometryType.Polygon);

            if (index[index.Count - 1] > 0)
                index.Add(0);
        }

        public void StartHole()
        {
            if (Type != GeometryType.Polygon)
                throw new ArgumentException($"Wrong type mode. Expected {GeometryType.Polygon}, but get {Type}.");

            if (index[index.Count - 1] > 0)
                index.Add(0);
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
