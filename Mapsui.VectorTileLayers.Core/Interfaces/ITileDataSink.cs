using Mapsui.VectorTileLayers.Core.Enums;
using Mapsui.VectorTileLayers.Core.Primitives;

namespace Mapsui.VectorTileLayers.Core.Interfaces
{
    public interface ITileDataSink
    {
        void Process(VectorElement element);

        /// <summary>
        /// Notify loader that tile loading is completed.
        /// </summary>
        /// <param name="result"></param>
        void Completed(QueryResult result);
    }
}
