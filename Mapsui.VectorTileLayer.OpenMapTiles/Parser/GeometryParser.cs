using Mapsui.VectorTileLayer.Core.Primitives;
using Mapsui.VectorTileLayer.OpenMapTiles.Pbf;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayer.OpenMapTiles.Parser
{
    public static class GeometryParser
    {
        /// <summary>
        /// Convert Mapbox tile format (see https://www.mapbox.com/vector-tiles/specification/)
        /// </summary>
        /// <param name="geom">Geometry information in Mapbox format</param>
        /// <param name="geomType">GeometryType of this geometry</param>
        /// <param name="scale">Factor for scaling of coordinates because of overzooming</param>
        /// <param name="offsetX">Offset in X direction because of overzooming</param>
        /// <param name="offsetY">Offset in Y direction because of overzooming</param>
        /// <returns>List of list of points in world coordinates</returns>
        public static List<List<MPoint>> ParseGeometry(List<uint> geom, GeomType geomType, Overzoom overzoom)
        {
            const uint cmdMoveTo = 1;
            //const uint cmdLineTo = 2;
            const uint cmdSegEnd = 7;
            //const uint cmdBits = 3;

            long x = 0;
            long y = 0;
            var listOfPoints = new List<List<MPoint>>();
            List<MPoint> points = null;
            var geometryCount = geom.Count;
            uint length = 0;
            uint command = 0;
            var i = 0;
            while (i < geometryCount)
            {
                if (length <= 0)
                {
                    length = geom[i++];
                    command = length & ((1 << 3) - 1);
                    length >>= 3;
                }

                if (length > 0)
                {
                    if (command == cmdMoveTo)
                    {
                        points = new List<MPoint>();
                        listOfPoints.Add(points);
                    }
                }

                if (command == cmdSegEnd)
                {
                    if (geomType != GeomType.Point && points?.Count != 0)
                    {
                        // It is a polygon, so add first point as last point to close
                        points?.Add(points[0]);
                    }
                    length--;
                    continue;
                }

                var dx = geom[i++];
                var dy = geom[i++];

                length--;

                var ldx = ZigZag.Decode(dx);
                var ldy = ZigZag.Decode(dy);

                x += ldx;
                y += ldy;

                // Correct coordinates for overzoom
                if (overzoom != Overzoom.None)
                {
                    points?.Add(new MPoint(x * overzoom.Scale - overzoom.OffsetX, y * overzoom.Scale - overzoom.OffsetY));
                }
                else
                {
                    points?.Add(new MPoint(x, y));
                }
            }
            return listOfPoints;
        }
    }
}