using Mapsui.VectorTileLayers.Core.Primitives;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayers.Core.Interfaces
{
    public interface IVectorSymbolFactory
    {
        bool HasIcon { get; }

        bool HasText { get; }

        Symbol CreateIconSymbol(MPoint point, float rotation, TagsCollection tags, EvaluationContext context);

        Symbol CreateTextSymbol(MPoint point, TagsCollection tags, EvaluationContext context);

        Symbol CreateIconTextSymbol(MPoint point, float rotation, TagsCollection tags, EvaluationContext context);

        IEnumerable<Symbol> CreatePathSymbols(VectorElement element, EvaluationContext context);
    }
}
