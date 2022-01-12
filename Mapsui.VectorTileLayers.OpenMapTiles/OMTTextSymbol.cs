using Mapsui.VectorTileLayers.Core.Primitives;
using Mapsui.VectorTileLayers.OpenMapTiles.Expressions;
using RBush;
using SkiaSharp;
using Topten.RichTextKit;

namespace Mapsui.VectorTileLayers.OpenMapTiles
{
    public class OMTTextSymbol : Symbol
    {
#if DEBUG
        SKPaint testPaint = new SKPaint { Color = SKColors.Red, Style = SKPaintStyle.Stroke, StrokeWidth = 0 };
#endif

        public OMTTextSymbol(TextBlock textBlock, Style textStyle)
        {
            TextBlock = textBlock;
            TextStyle = textStyle;
        }

        public Style TextStyle { get; }

        public TextBlock TextBlock { get; }

        public bool TextOptional { get; set; }

        public StoppedFloat TextHaloBlur { get; set; } = new StoppedFloat() { SingleVal = 0 };

        public StoppedColor TextHaloColor { get; set; } = new StoppedColor() { SingleVal = new SKColor(0, 0, 0, 0) };

        public StoppedFloat TextHaloWidth { get; set; } = new StoppedFloat() { SingleVal = 0 };

        public override void AddEnvelope(RBush<Symbol> tree)
        {
            tree.Insert(this);
        }

        public override void CalcEnvelope(float scale, float rotation)
        {
            // Convert tile coordinates to pixel
            var newPoint = new MPoint(Point.X * scale, Point.Y * scale);
            // Add anchor and offset in pixel
            newPoint.X += Anchor.X + Offset.X;
            newPoint.Y += Anchor.Y + Offset.Y;
            // Add real size in pixel
            var width = TextBlock.MeasuredWidth;
            var height = TextBlock.MeasuredHeight;
            var minX = newPoint.X - Padding; // + TextBlock.MeasuredPadding.Left - Padding;
            var minY = newPoint.Y - Padding; // + TextBlock.MeasuredPadding.Top - Padding;
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
            if (!IsVisible)
                return;

            if (Name != null)
            {
                canvas.Save();
                canvas.Translate((float)Point.X, (float)Point.Y);
                canvas.Scale(context.Scale, context.Scale);
                // TextBlock.Paint draws always with MaxWidth bounds
                canvas.Translate((float)Anchor.X + (float)Offset.X - TextBlock.MeasuredPadding.Left, (float)Anchor.Y + (float)Offset.Y - TextBlock.MeasuredPadding.Top);
                var paint = Paint.CreatePaint(context);
                TextStyle.TextColor = paint.Color;
                TextStyle.HaloBlur = (float)TextHaloBlur.Evaluate(context);
                TextStyle.HaloColor = (SKColor)TextHaloColor.Evaluate(context);
                TextStyle.HaloWidth = (float)TextHaloWidth.Evaluate(context);
                TextBlock.Paint(canvas);
                canvas.Restore();

#if DEBUG
                // TODO: Only for testing
                //if (Name == "Chapiteau de Fontvieille" || Name.StartsWith("Post") || Name.StartsWith("Caval"))
                {
                    canvas.DrawRect(new SKRect((float)_envelope.MinX, (float)_envelope.MinY, (float)_envelope.MaxX, (float)_envelope.MaxY), testPaint);
                }
#endif
            }
        }

        public override Symbol TreeSearch(RBush<Symbol> tree)
        {
            var resultText = tree.Search(Envelope);
            var drawText = true;

            foreach (var foundForText in resultText)
            {
                // Both symbols could occupy the same place
                if (IgnorePlacement && foundForText.IgnorePlacement)
                    continue;

                drawText = false;
            }

            if (!drawText)
            {
                // We couldn't draw the text, but it isn't optional. So we draw nothing
                return null;
            }

            return this;

        }
    }
}
