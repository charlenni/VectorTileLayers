﻿using Mapsui.VectorTileLayers.Core.Enums;
using Mapsui.VectorTileLayers.Core.Interfaces;
using SkiaSharp;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayers.Core.Primitives
{
    public class SymbolBucket : IBucket
    {
        IVectorStyle _styleLayer;
        IVectorSymbolFactory _styler;

        public List<Symbol> Symbols = new List<Symbol>();

        public SymbolBucket(IVectorStyle styleLayer)
        {
            _styleLayer = styleLayer;
            _styler = styleLayer.SymbolStyler;
        }

        public void AddElement(VectorElement element, EvaluationContext context = null)
        {
            switch (element.Type)
            {
                case GeometryType.Point:
                    foreach (var point in element.Points)
                    {
                        if (_styler.HasIcon && _styler.HasText)
                        {
                            var iconTextSymbol = _styler.CreateIconTextSymbol(point, 0f, element.Tags, context);
                            if (iconTextSymbol != null)
                            {
                                iconTextSymbol.IsVisible = _styleLayer.Enabled;
                                iconTextSymbol.Index = element.TileIndex;
                                iconTextSymbol.Style = _styleLayer;
                                Symbols.Add(iconTextSymbol);
                            }
                        }
                        else if (_styler.HasIcon)
                        {
                            var iconSymbol = _styler.CreateIconSymbol(point, 0f, element.Tags, context);
                            if (iconSymbol != null)
                            {
                                iconSymbol.IsVisible = _styleLayer.Enabled;
                                iconSymbol.Index = element.TileIndex;
                                iconSymbol.Style = _styleLayer;
                                Symbols.Add(iconSymbol);
                            }
                        }
                        else if (_styler.HasText)
                        {
                            var textSymbol = _styler.CreateTextSymbol(point, element.Tags, context);
                            if (textSymbol != null)
                            {
                                textSymbol.IsVisible = _styleLayer.Enabled;
                                textSymbol.Index = element.TileIndex;
                                textSymbol.Style = _styleLayer;
                                Symbols.Add(textSymbol);
                            }
                        }
                    }
                    break;
                case GeometryType.LineString:
                    if (_styler == null)
                        return;
                    var pathSymbols = _styler.CreatePathSymbols(element, context);
                    if (pathSymbols != null)
                    {
                        foreach (var pathSymbol in pathSymbols)
                        {
                            pathSymbol.Style = _styleLayer;
                            Symbols.Add(pathSymbol);
                        }
                    }
                    break;
                case GeometryType.Polygon:
                    var t3 = _styleLayer.SourceLayer;
                    break;
                default:
                    var t4 = _styleLayer.SourceLayer;
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
