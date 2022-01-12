using SkiaSharp;
using Mapsui.VectorTileLayers.Core.Primitives;

namespace Mapsui.VectorTileLayers.Core.Interfaces
{
    public interface IVectorPaint
    {
        /// <summary>
        /// Creates a SKPaint to 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        SKPaint CreatePaint(EvaluationContext context);
    }
}
