using Mapsui.VectorTileLayer.Core.Interfaces;
using Mapsui.VectorTileLayer.Core.Primitives;
using System;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayer.OpenMapTiles.Expressions
{
    /// <summary>
    /// Class holding StoppedFloat data in Json format
    /// </summary>
    public class StoppedFloat : IExpression
    {
        public float Base { get; set; } = 1f;

        public IList<KeyValuePair<float, float>> Stops { get; set; }

        public float SingleVal { get; set; } = float.MinValue;

        /// <summary>
        /// Calculate the correct value for a stopped function
        /// No Bezier type up to now
        /// </summary>
        /// <param name="contextZoom">Zoom factor for calculation </param>
        /// <param name="stoppsType">Type of calculation (interpolate, exponential, categorical)</param>
        /// <returns>Value for this stopp respecting zoom factor and type</returns>
        public float Evaluate(float? contextZoom, StopsType stoppsType = StopsType.Exponential)
        {
            // Are there no stopps, but a single value?
            // !=
            if (SingleVal - float.MinValue > float.Epsilon)
                return SingleVal;

            // Are there no stopps in array
            if (Stops.Count == 0)
                return 0;

            float zoom = contextZoom ?? 0f;

            var lastZoom = Stops[0].Key;
            var lastValue = Stops[0].Value;

            if (lastZoom > zoom)
                return lastValue;

            for (int i = 1; i < Stops.Count; i++)
            {
                var nextZoom = Stops[i].Key;
                var nextValue = Stops[i].Value;

                if (zoom == nextZoom)
                    return nextValue;

                if (lastZoom <= zoom && zoom < nextZoom)
                {
                    switch (stoppsType)
                    {
                        case StopsType.Interval:
                            return lastValue;
                        case StopsType.Exponential:
                            var progress = zoom - lastZoom;
                            var difference = nextZoom - lastZoom;
                            if (difference < float.Epsilon)
                                return 0;
                            if (Base - 1.0f < float.Epsilon)
                                return lastValue + (nextValue - lastValue) * progress / difference;
                            else
                            {
                                //var r = FromResolution(resolution);
                                //var lr = FromResolution(lastResolution);
                                //var nr = FromResolution(nextResolution);
                                //var logBase = Math.Log(Base);
                                //return lastValue + (float)((nextValue - lastValue) * (Math.Pow(Base, lr-r) - 1) / (Math.Pow(Base, lr-nr) - 1));
                                //return lastValue + (float)((nextValue - lastValue) * (Math.Exp(progress * logBase) - 1) / (Math.Exp(difference * logBase) - 1)); // (Math.Pow(Base, progress) - 1) / (Math.Pow(Base, difference) - 1));
                                return lastValue + (float)((nextValue - lastValue) * (Math.Pow(Base, progress) - 1) / (Math.Pow(Base, difference) - 1));
                            }
                        case StopsType.Categorical:
                            if (nextZoom - zoom < float.Epsilon)
                                return nextValue;
                            break;
                    }
                }

                lastZoom = nextZoom;
                lastValue = nextValue;
            }

            return lastValue;
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
