using System;

namespace Mapsui.VectorTileLayers.Core.Extensions
{
    public static class ZoomExtensions
    {
        private const double ScaleFactor = 78271.51696401953125;

        private static readonly double[] Resolutions;

        static ZoomExtensions()
        {
            Resolutions = new double[31];

            for (int i = 0; i <= 30; i++)
                Resolutions[i] = 2 * ScaleFactor / (1 << i);
        }

        public static double ToResolution(this float zoom)
        {
            return 2 * ScaleFactor / Math.Pow(2, zoom);
        }

        public static double ToResolution(this int level)
        {
            return 2 * ScaleFactor / (1 << level);
        }

        public static double ToZoomLevel(this double resolution)
        {
            return Math.Log(2 * ScaleFactor / resolution, 2);
        }
    }
}
