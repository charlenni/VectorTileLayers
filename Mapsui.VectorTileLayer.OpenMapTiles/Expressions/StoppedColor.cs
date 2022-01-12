using Mapsui.VectorTileLayer.Core.Interfaces;
using Mapsui.VectorTileLayer.Core.Primitives;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayer.OpenMapTiles.Expressions
{
    /// <summary>
    /// Class holding StoppedColor data
    /// </summary>
    public class StoppedColor : IExpression
    {
        public float Base { get; set; } = 1f;

        public IList<KeyValuePair<float, SKColor>> Stops { get; set; }

        public SKColor? SingleVal { get; set; }

        /// <summary>
        /// Calculate the correct color for a stopped function
        /// No Bezier type up to now
        /// </summary>
        /// <param name="contextZoom">Zoom factor for calculation </param>
        /// <param name="stoppsType">Type of calculation (interpolate, exponential, categorical)</param>
        /// <returns>Value for this stopp respecting zoom factor and type</returns>
        public SKColor Evaluate(float? contextZoom, StopsType stoppsType = StopsType.Exponential)
        {
            // Are there no stopps, but a single value?
            if (SingleVal != null)
                return (SKColor)SingleVal;

            // Are there no stopps in array
            if (Stops.Count == 0)
                return SKColors.Empty;

            float zoom = contextZoom ?? 0f;

            var lastZoom = Stops[0].Key;
            var lastColor = Stops[0].Value;

            if (lastZoom > zoom)
                return lastColor;

            for (int i = 1; i < Stops.Count; i++)
            {
                var nextZoom = Stops[i].Key;
                var nextColor = Stops[i].Value;

                if (zoom == nextZoom)
                    return nextColor;

                if (lastZoom <= zoom && zoom < nextZoom)
                {
                    switch (stoppsType)
                    {
                        case StopsType.Interval:
                            return lastColor;
                        case StopsType.Exponential:
                            var progress = zoom - lastZoom;
                            var difference = nextZoom - lastZoom;
                            if (difference < float.Epsilon)
                                return SKColors.Empty;
                            float factor;
                            if (Base - 1 < float.Epsilon)
                                factor = progress / difference;
                            else
                                factor = (float)((Math.Pow(Base, progress) - 1) / (Math.Pow(Base, difference) - 1));
                            var r = (byte)Math.Round(lastColor.Red + (nextColor.Red - lastColor.Red) * factor);
                            var g = (byte)Math.Round(lastColor.Green + (nextColor.Green - lastColor.Green) * factor);
                            var b = (byte)Math.Round(lastColor.Blue + (nextColor.Blue - lastColor.Blue) * factor);
                            var a = (byte)Math.Round(lastColor.Alpha + (nextColor.Alpha - lastColor.Alpha) * factor);
                            return new SKColor(r, g, b, a);
                        case StopsType.Categorical:
                            // ==
                            if (nextZoom - zoom < float.Epsilon)
                                return nextColor;
                            break;
                    }
                }

                lastZoom = nextZoom;
                lastColor = nextColor;
            }

            return lastColor;
        }

        public object Evaluate(EvaluationContext ctx)
        {
            return Evaluate(ctx.Zoom, StopsType.Exponential);
        }

        public object PossibleOutputs()
        {
            throw new NotImplementedException();
        }
    }
}
