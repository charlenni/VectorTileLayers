using SkiaSharp;
using Mapsui.VectorTileLayer.Core.Primitives;

namespace Mapsui.VectorTileLayer.Core.Interfaces
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
