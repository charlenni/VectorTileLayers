using Mapsui.VectorTileLayer.Core.Enums;
using Mapsui.VectorTileLayer.Core.Primitives;

namespace Mapsui.VectorTileLayer.Core.Interfaces
{
    public interface IVectorElement
    {
        string Id { get; }

        GeometryType Type { get; }

        TagsCollection Tags { get; }
    }
}
