using Mapsui.VectorTileLayer.Core.Primitives;
using RBush;
using SkiaSharp;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayer.OpenMapTiles
{
    public class OMTIconTextSymbol : Symbol
    {
        public OMTIconSymbol IconSymbol { get; }

        public OMTTextSymbol TextSymbol { get; }

        public OMTIconTextSymbol(OMTIconSymbol icon, OMTTextSymbol text)
        {
            IconSymbol = icon;
            TextSymbol = text;
        }

        public override void AddEnvelope(RBush<Symbol> tree)
        {
            if (IconSymbol != null)
                tree.Insert(IconSymbol);
            if (TextSymbol != null)
                tree.Insert(TextSymbol);
        }

        public override void CalcEnvelope(float scale, float rotation)
        {
            IconSymbol?.CalcEnvelope(scale, rotation);
            TextSymbol?.CalcEnvelope(scale, rotation);
        }

        public override void Draw(SKCanvas canvas, EvaluationContext context)
        {
            IconSymbol?.Draw(canvas, context);
            TextSymbol?.Draw(canvas, context);
        }

        public override Symbol TreeSearch(RBush<Symbol> tree)
        {
            var drawIcon = IconSymbol != null;
            var drawText = TextSymbol != null;

            if (drawIcon)
            {
                var resultIcon = tree.Search(IconSymbol.Envelope);

                foreach (var foundForIcon in resultIcon)
                {
                    // Both symbols could occupy the same place
                    if (IconSymbol.IgnorePlacement && foundForIcon.IgnorePlacement)
                        continue;

                    drawIcon = false;
                }

                if (!drawIcon && !IconSymbol.IconOptional)
                {
                    // We couldn't draw the icon, but it isn't optional. So we draw nothing
                    return null;
                }
            }

            if (drawText)
            {
                var resultText = tree.Search(TextSymbol.Envelope);

                foreach (var foundForText in resultText)
                {
                    // Both symbols could occupy the same place
                    if (TextSymbol.IgnorePlacement && foundForText.IgnorePlacement)
                        continue;

                    drawText = false;
                }

                if (!drawText && !TextSymbol.TextOptional)
                {
                    // We couldn't draw the text, but it isn't optional. So we draw nothing
                    return null;
                }
            }

            if (drawIcon && !drawText)
            {
                // We can only draw icon
                return IconSymbol;
            }

            if (!drawIcon && drawText)
            {
                // We can only draw text
                return TextSymbol;
            }

            if (drawIcon && drawText)
            {
                // Draw both
                return this;
            }

            return null;
        }
    }
}
