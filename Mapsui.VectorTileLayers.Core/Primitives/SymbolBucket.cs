using Mapsui.VectorTileLayers.Core.Enums;
using Mapsui.VectorTileLayers.Core.Interfaces;
using SkiaSharp;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayers.Core.Primitives
{
    public class SymbolBucket : IBucket
    {
        IStyleLayer styleLayer;
        IVectorSymbolFactory styler;

        public List<Symbol> Symbols = new List<Symbol>();

        public SymbolBucket(IStyleLayer style)
        {
            styleLayer = style;
            styler = style.SymbolStyler;
        }

        public void AddElement(VectorElement element, EvaluationContext context = null)
        {
            switch (element.Type)
            {
                case GeometryType.Point:
                    foreach (var point in element.Points)
                    {
                        if (styler.HasIcon && styler.HasText)
                        {
                            var iconTextSymbol = styler.CreateIconTextSymbol(point, 0f, element.Tags, context);
                            if (iconTextSymbol != null)
                            {
                                iconTextSymbol.IsVisible = styleLayer.Enabled;
                                iconTextSymbol.Index = element.TileIndex;
                                Symbols.Add(iconTextSymbol);
                            }
                        }
                        else if (styler.HasIcon)
                        {
                            var iconSymbol = styler.CreateIconSymbol(point, 0f, element.Tags, context);
                            if (iconSymbol != null)
                            {
                                iconSymbol.IsVisible = styleLayer.Enabled;
                                iconSymbol.Index = element.TileIndex;
                                Symbols.Add(iconSymbol);
                            }
                        }
                        else if (styler.HasText)
                        {
                            var textSymbol = styler.CreateTextSymbol(point, element.Tags, context);
                            if (textSymbol != null)
                            {
                                textSymbol.IsVisible = styleLayer.Enabled;
                                textSymbol.Index = element.TileIndex;
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
                    {
                        Symbols.AddRange(pathSymbol);
                    }
                    break;
                case GeometryType.Polygon:
                    var t3 = styleLayer.SourceLayer;
                    break;
                default:
                    var t4 = styleLayer.SourceLayer;
                    break;
            }
        }

        public void Dispose()
        {
            foreach (var symbol in Symbols)
                symbol.Dispose();
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
