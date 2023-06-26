using BruTile;
using Mapsui.VectorTileLayers.Core.Primitives;
using RBush;
using SkiaSharp;

namespace Mapsui.VectorTileLayers.OpenMapTiles
{
    public class OMTPathSymbol : Symbol
    {
        public TileIndex TileIndex { get; private set; }

        public SKPath Path { get; }

        public OMTPathSymbol(TileIndex tileIndex, string id)
        {
            TileIndex = tileIndex;
            Id = id;
            Path = new SKPath();
        }

        public override void Update(EvaluationContext context)
        {
        }

        public override void AddEnvelope(RBush<Symbol> tree)
        {
            //throw new System.NotImplementedException();
        }

        public override void CalcEnvelope(float scale, float rotation, MPoint offset)
        {
            //throw new NotImplementedException();
        }

        public override void Render(RBush<Symbol> tree, EvaluationContext context)
        {
            // throw new System.NotImplementedException();
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
