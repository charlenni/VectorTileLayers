using Mapsui.VectorTileLayers.Core.Enums;
using Mapsui.VectorTileLayers.Core.Primitives;

namespace Mapsui.VectorTileLayers.Core.Interfaces
{
    public interface IVectorElement
    {
        string Id { get; }

        GeometryType Type { get; }

        TagsCollection Tags { get; }
    }
}
