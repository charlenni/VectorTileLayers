using Mapsui.VectorTileLayer.Core.Enums;
using Mapsui.VectorTileLayer.Core.Primitives;

namespace Mapsui.VectorTileLayer.Core.Interfaces
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
