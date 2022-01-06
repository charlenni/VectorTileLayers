using Mapsui.VectorTileLayer.Core.Enums;
using Mapsui.VectorTileLayer.Core.Interfaces;
using SkiaSharp;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayer.Core.Primitives
{
    public class SymbolBucket : IBucket
    {
        IVectorStyleLayer styleLayer;
        IVectorSymbolStyler styler;

        public List<Symbol> Symbols = new List<Symbol>();

        public SymbolBucket(IVectorStyleLayer style)
        {
            styleLayer = style;
            styler = style.SymbolStyler;
        }

        public void AddElement(VectorElement element, EvaluationContext context = null)
        {
            // TODO: Remove, is only for tests
            if (styleLayer.SourceLayer == "poi")
            {
                var t10 = 10;
            }

            switch (element.Type)
            {
                case GeometryType.Point:
                    foreach (var point in element.Points)
                    {
                        if (styler.HasIcon && styler.HasText)
                        {
                            var iconTextSymbol = styler.CreateIconTextSymbol(point, element.Tags, context);
                            if (iconTextSymbol != null)
                            {
                                iconTextSymbol.IsVisible = styleLayer.IsVisible;
                                Symbols.Add(iconTextSymbol);
                            }
                        }
                        else if (styler.HasIcon)
                        {
                            var iconSymbol = styler.CreateIconSymbol(point, element.Tags, context);
                            if (iconSymbol != null)
                            {
                                iconSymbol.IsVisible = styleLayer.IsVisible;
                                Symbols.Add(iconSymbol);
                            }
                        }
                        else if (styler.HasText)
                        {
                            var textSymbol = styler.CreateTextSymbol(point, element.Tags, context);
                            if (textSymbol != null)
                            {
                                textSymbol.IsVisible = styleLayer.IsVisible;
                                Symbols.Add(textSymbol);
                            }
                        }
                    }
                    break;
                case GeometryType.LineString:
                    if (styler == null)
                        return;
                    var pathSymbol = styler.CreatePathSymbols(element, context);
                    if (pathSymbol != null)
                        Symbols.Add(pathSymbol);
                    break;
                case GeometryType.Polygon:
                    var t3 = styleLayer.SourceLayer;
                    break;
                default:
                    var t4 = styleLayer.SourceLayer;
                    break;
            }
        }

        public void OnDraw(SKCanvas canvas, EvaluationContext context)
        {
            foreach (var symbol in Symbols)
            {
                symbol.Draw(canvas, context);
            }
        }
    }
}
