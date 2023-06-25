using Mapsui.VectorTileLayers.Core.Enums;
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
        SKPath testPath;
#endif
        public SKImage Image { get; set; }

        public bool IconOptional { get; set; }

        public float IconSize { get; set; } = 1;

        public MPoint Translate { get; set; }

        public MapAlignment TranslateAnchor { get; set; } = MapAlignment.Map;

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
            var newPoint = Point.Copy(); // new MPoint(Point.X * scale, Point.Y * scale);
            // Add anchor and offset in pixel
            newPoint.X += (PossibleAnchors[0].X + Offset.X) / scale;
            newPoint.Y += (PossibleAnchors[0].Y + Offset.Y) / scale;
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
            var matrix = CreateMatrix(scale, 0);
            var tl = matrix.MapPoint(new SKPoint(0, 0));
            var tr = matrix.MapPoint(new SKPoint(Image.Width, 0));
            var bl = matrix.MapPoint(new SKPoint(0, Image.Height));
            var br = matrix.MapPoint(new SKPoint(Image.Width, Image.Height));

            testPath = new SKPath();
            testPath.MoveTo(tl);
            testPath.LineTo(tr);
            testPath.LineTo(br);
            testPath.LineTo(bl);
            testPath.Close();

            testRect = new SKRect(minX, minY, maxX, maxY);
#endif
        }

        public override void Render(RBush<Symbol> tree, EvaluationContext context)
        {
            if (Image == null)
                return;

            if (TranslateAnchor == MapAlignment.Map)
            {

            }

            // Image width and height in pixel
            var width = Image.Width * IconSize + Padding;
            var height = Image.Height * IconSize + Padding;

            // Add offset in pixel
            var posX = (float)Offset.X * IconSize;
            var posY = (float)Offset.Y * IconSize;
            
            posX += (float)Anchor.X;
            posY += (float)Anchor.Y;

            // Calc envelop of image
            var envelopes = new Envelope();


            // Get all possible positions
            foreach (var anchor in PossibleAnchors)
            {
                
            }
        }

        public override void Draw(SKCanvas canvas, EvaluationContext context)
        {
            if (!IsVisible || Image == null)
                return;

            canvas.Save();

            canvas.Translate((float)Point.X, (float)Point.Y);
            canvas.Scale(context.Scale, context.Scale);
            if (Alignment == Core.Enums.MapAlignment.Viewport)
                canvas.RotateDegrees(-context.Rotation);
            // Offset could be in relation to Map or Viewport
            canvas.Translate((float)Offset.X, (float)Offset.Y);
            canvas.Scale(IconSize);
            canvas.RotateDegrees(Rotation);
            canvas.Translate((float)Anchor.X, (float)Anchor.Y);
            //canvas.SetMatrix(canvas.TotalMatrix.PreConcat(CreateMatrix(context.Scale * IconSize, context.Rotation)));
            var paint = Paint.CreatePaint(context);
            canvas.DrawImage(Image, 0, 0, paint);

#if DEBUG
            //canvas.DrawPath(testPath, testPaint);
            //canvas.DrawRect(testRect, testPaint);
#endif

            canvas.Restore();
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

        private SKMatrix CreateMatrix(float scale, float rotation)
        {
            SKMatrix result = SKMatrix.Identity;

            result = result.PreConcat(SKMatrix.CreateTranslation((float)Point.X, (float)Point.Y));
            result = result.PreConcat(SKMatrix.CreateScale(scale, scale));
            if (Alignment == Core.Enums.MapAlignment.Viewport)
                result = result.PreConcat(SKMatrix.CreateRotationDegrees(-rotation));
            result = result.PreConcat(SKMatrix.CreateTranslation((float)Anchor.X, (float)Anchor.Y));
            // Offset could be in relation to Map or Viewport
            result = result.PreConcat(SKMatrix.CreateTranslation((float)Offset.X, (float)Offset.Y));
            result = result.PreConcat(SKMatrix.CreateScale(IconSize, IconSize));

            return result;
        }
    }
}
