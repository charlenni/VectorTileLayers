using Mapsui.VectorTileLayers.Core.Primitives;
using RBush;
using SkiaSharp;

namespace Mapsui.VectorTileLayers.OpenMapTiles
{
    public class OMTIconSymbol : Symbol
    {
#if DEBUG
        // TODO: Only for testing
        SKPaint testPaint = new SKPaint { Color = SKColors.Red, Style = SKPaintStyle.Stroke, StrokeWidth = 0 };
        SKRect testRect;
#endif
        public SKImage Image { get; set; }

        public bool IconOptional { get; set; }

        public float IconSize { get; set; } = 1;

        public override void Update(EvaluationContext context)
        {
        }

        public override void AddEnvelope(RBush<Symbol> tree)
        {
            tree.Insert(this);
        }

        public override void CalcEnvelope(float scale, float rotation, MPoint offset)
        {
            if (Image == null)
            {
                _envelope = Envelope.EmptyBounds;
                return;
            }

            // Convert tile coordinates to pixel
            var newPoint = Point.Clone(); // new MPoint(Point.X * scale, Point.Y * scale);
            // Add anchor and offset in pixel
            newPoint.X += (Anchor.X + Offset.X) / scale;
            newPoint.Y += (Anchor.Y + Offset.Y) / scale;
            // Add real size in pixel
            var width = Image.Width * IconSize / scale;
            var height = Image.Height * IconSize / scale;
            var padding = Padding / scale;
            var minX = (float)(newPoint.X - padding);
            var minY = (float)(newPoint.Y - padding);
            var maxX = (float)(minX + width + padding * 2);
            var maxY = (float)(minY + height + padding * 2);
            // Create envelope
            _envelope = new Envelope(minX + offset.X, minY + offset.Y, maxX + offset.X, maxY + offset.Y);
#if DEBUG
            testRect = new SKRect(minX, minY, maxX, maxY);
#endif
        }

        public override void Draw(SKCanvas canvas, EvaluationContext context)
        {
            if (!IsVisible || Image == null)
                return;

            canvas.Save();
            canvas.Translate((float)Point.X, (float)Point.Y);
            canvas.Scale(context.Scale, context.Scale);
            canvas.Translate((float)Anchor.X, (float)Anchor.Y);
            // Offset could be in relation to Map or Viewport
            canvas.Translate((float)Offset.X, (float)Offset.Y);
            var paint = Paint.CreatePaint(context);
            canvas.Scale(IconSize);
            canvas.DrawImage(Image, 0, 0, paint);
            canvas.Restore();

#if DEBUG
            canvas.DrawRect(testRect, testPaint);
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
