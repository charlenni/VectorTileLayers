using Mapsui.VectorTileLayer.Core.Primitives;
using RBush;
using SkiaSharp;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayer.MapboxGL
{
    public class MGLIconSymbol : Symbol
    {
#if DEBUG
        // TODO: Only for testing
        SKPaint testPaint = new SKPaint { Color = SKColors.Red, Style = SKPaintStyle.Stroke, StrokeWidth = 0 };
#endif
        public SKImage Image { get; set; }

        public bool IconOptional { get; set; }

        public float IconSize { get; set; } = 1;

        public override void AddEnvelope(RBush<Symbol> tree)
        {
            tree.Insert(this);
        }

        public override void CalcEnvelope(float scale, float rotation)
        {
            if (Image == null)
            {
                _envelope = Envelope.EmptyBounds;
                return;
            }

            // Convert tile coordinates to pixel
            var newPoint = new MPoint(Point.X * scale, Point.Y * scale);
            // Add anchor and offset in pixel
            newPoint.X += Anchor.X + Offset.X;
            newPoint.Y += Anchor.Y + Offset.Y;
            // Add real size in pixel
            var width = Image.Width * IconSize;
            var height = Image.Height * IconSize;
            var minX = newPoint.X - Padding;
            var minY = newPoint.Y - Padding;
            var maxX = minX + width + Padding * 2;
            var maxY = minY + height + Padding * 2;
            // Convert back in tile coordinates
            minX /= scale;
            minY /= scale;
            maxX /= scale;
            maxY /= scale;
            // Create envelope
            _envelope = new Envelope(minX, minY, maxX, maxY);
        }

        public override void Draw(SKCanvas canvas, EvaluationContext context)
        {
            if (!IsVisible || Image == null)
                return;

            canvas.Save();
            canvas.Translate((float)Point.X, (float)Point.Y);
            canvas.Scale(1 / canvas.TotalMatrix.ScaleX, 1 / canvas.TotalMatrix.ScaleY);
            canvas.Translate((float)Anchor.X, (float)Anchor.Y);
            // Offset could be in relation to Map or Viewport
            canvas.Translate((float)Offset.X, (float)Offset.Y);
            var paint = Paint.CreatePaint(context);
            canvas.Scale(IconSize);
            canvas.DrawImage(Image, 0, 0, paint);
            canvas.Restore();

#if DEBUG
            // TODO: Only for testing
            canvas.DrawRect(new SKRect((float)_envelope.MinX, (float)_envelope.MinY, (float)_envelope.MaxX, (float)_envelope.MaxY), testPaint);
#endif
        }

        public override Symbol TreeSearch(RBush<Symbol> tree)
        {
            var resultIcon = tree.Search(Envelope);
            var drawIcon = true;

            foreach (var foundForIcon in resultIcon)
            {
                // Both symbols could occupy the same place
                if (IgnorePlacement && foundForIcon.IgnorePlacement)
                    continue;

                drawIcon = false;
            }

            if (!drawIcon)
            {
                // We couldn't draw the icon
                return null;
            }

            return this;
        }
    }
}
