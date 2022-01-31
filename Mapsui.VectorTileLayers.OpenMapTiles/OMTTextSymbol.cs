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
        SKPaint testPaint = new SKPaint { Color = SKColors.Blue, Style = SKPaintStyle.Stroke, StrokeWidth = 0 };
        SKRect testRect;
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

        public override void Update(EvaluationContext context)
        {
        }

        public override void AddEnvelope(RBush<Symbol> tree)
        {
            tree.Insert(this);
        }

        public override void CalcEnvelope(float scale, float rotation, MPoint offset)
        {
            // Convert tile coordinates to pixel
            var newPoint = Point.Clone(); // new MPoint(Point.X * scale, Point.Y * scale);
            // Add anchor and offset in pixel
            newPoint.X += (Anchor.X + Offset.X) / scale;
            newPoint.Y += (Anchor.Y + Offset.Y) / scale;
            // Add real size in pixel
            var width = TextBlock.MeasuredWidth / scale;
            var height = TextBlock.MeasuredHeight / scale;
            var padding = Padding / scale;
            var minX = (float)newPoint.X - padding;
            var minY = (float)newPoint.Y - padding;
            var maxX = (float)minX + width + padding * 2;
            var maxY = (float)minY + height + padding * 2;
            // Create envelope
            _envelope = new Envelope(minX + offset.X, minY + offset.Y, maxX + offset.X, maxY + offset.Y);
#if DEBUG
            testRect = new SKRect(minX, minY, maxX, maxY);
#endif
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
                //if (Alignment == Core.Enums.MapAlignment.Viewport)
                //    canvas.RotateDegrees(context.)
                var paint = Paint.CreatePaint(context);
                TextStyle.TextColor = paint.Color;
                TextStyle.HaloBlur = (float)TextHaloBlur.Evaluate(context);
                TextStyle.HaloColor = (SKColor)TextHaloColor.Evaluate(context);
                TextStyle.HaloWidth = (float)TextHaloWidth.Evaluate(context);
                TextBlock.Paint(canvas);
                canvas.Restore();

#if DEBUG
                //if (Name.StartsWith("FONTVIEILLE") || Name == "Chapiteau de Fontvieille" || Name.StartsWith("Post") || Name.StartsWith("Caval"))
                    canvas.DrawRect(testRect, testPaint);
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
