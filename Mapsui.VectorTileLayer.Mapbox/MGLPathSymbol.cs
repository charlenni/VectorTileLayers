using BruTile;
using Mapsui.VectorTileLayer.Core.Primitives;
using RBush;
using SkiaSharp;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayer.MapboxGL
{
    public class MGLPathSymbol : Symbol
    {
        public TileIndex TileIndex { get; private set; }

        public MGLPathSymbol(TileIndex tileIndex, string id)
        {
            TileIndex = tileIndex;
            Id = id;
        }

        public override void AddEnvelope(RBush<Symbol> tree)
        {
            //throw new System.NotImplementedException();
        }

        public override void CalcEnvelope(float scale, float rotation)
        {
            //throw new NotImplementedException();
        }

        public override void Draw(SKCanvas canvas, EvaluationContext context)
        {
            //throw new System.NotImplementedException();
        }

        public override Symbol TreeSearch(RBush<Symbol> tree)
        {
            //throw new System.NotImplementedException();
            return this;
        }
    }
}
