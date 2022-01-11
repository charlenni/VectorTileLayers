using Mapsui.VectorTileLayer.Core.Enums;
using Mapsui.VectorTileLayer.Core.Interfaces;
using Mapsui.VectorTileLayer.Core.Primitives;
using Mapsui.VectorTileLayer.Mapbox.Extensions;
using Mapsui.VectorTileLayer.MapboxGL.Expressions;
using SkiaSharp;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Topten.RichTextKit;

namespace Mapsui.VectorTileLayer.MapboxGL
{
    public class MGLSymbolStyler : IVectorSymbolStyler
    {
        static Regex regex = new Regex(@".*\{(.*)\}.*");

        public static Dictionary<string, SKTypeface> SpecialFonts = new Dictionary<string, SKTypeface>();

        MGLSpriteAtlas spriteAtlas;
        Style textStyle;

        public static MGLSymbolStyler Default;

        public MGLSymbolStyler(MGLSpriteAtlas atlas)
        {
            spriteAtlas = atlas;
        }

        public bool HasIcon { get => IconImage != null; }

        public bool HasText { get => TextField != null && TextFont != null; }

        public bool IsVisible { get; internal set; } = true;

        public bool IconAllowOverlap { get; internal set; }

        public Direction IconAnchor { get; internal set; } = Direction.Center;

        public StoppedColor IconColor { get; internal set; } = new StoppedColor() { SingleVal = new SKColor(0, 0, 0, 0) };

        public StoppedFloat IconHaloBlur { get; internal set; } = new StoppedFloat() { SingleVal = 0 };

        public StoppedColor IconHaloColor { get; internal set; } = new StoppedColor() { SingleVal = new SKColor(0, 0, 0, 0) };

        public StoppedFloat IconHaloWidth { get; internal set; } = new StoppedFloat() { SingleVal = 0 };

        public bool IconIgnorePlacement { get; internal set; } = false;

        public StoppedString IconImage { get; internal set; }

        public bool IconKeepUpright { get; internal set; }

        public Vector IconOffset { get; internal set; } = Vector.Empty;

        public StoppedFloat IconOpacity { get; internal set; } = new StoppedFloat() { SingleVal = 1 };

        public bool IconOptional { get; internal set; } = false;

        public StoppedFloat IconPadding { get; internal set; } = new StoppedFloat() { SingleVal = 2 };

        public MapAlignment IconPitchAlignment { get; internal set; } = MapAlignment.Auto;

        public StoppedFloat IconRotate { get; internal set; } = new StoppedFloat() { SingleVal = 0 };

        public MapAlignment IconRotationAlignment { get; internal set; } = MapAlignment.Auto;

        public StoppedFloat IconSize { get; internal set; } = new StoppedFloat() { SingleVal = 1 };

        public TextFit IconTextFit { get; internal set; } = TextFit.None;

        public MRect IconTextFitPadding { get; internal set; } = new MRect(0, 0, 0, 0);

        public Vector IconTranslate { get; internal set; } = Vector.Empty;

        public MapAlignment IconTranslateAnchor { get; internal set; } = MapAlignment.Map;

        public bool SymbolAvoidEdges { get; internal set; }

        public StoppedString SymbolPlacement { get; internal set; } = new StoppedString() { SingleVal = "point" };

        public float SymbolSortKey { get; internal set; }

        public StoppedFloat SymbolSpacing { get; internal set; } = new StoppedFloat() { SingleVal = 250 };

        public ZOrder SymbolZOrder { get; internal set; } = ZOrder.Auto;

        public bool TextAllowOverlap { get; internal set; }

        public Direction TextAnchor { get; internal set; } = Direction.Center;

        public StoppedColor TextColor { get; internal set; } = new StoppedColor() { SingleVal = new SKColor(0, 0, 0, 0) };

        public string TextField { get; internal set; } = "";

        public List<string> TextFont { get; internal set; } = new List<string>();

        public StoppedFloat TextHaloBlur { get; internal set; } = new StoppedFloat() { SingleVal = 0 };

        public StoppedColor TextHaloColor { get; internal set; } = new StoppedColor() { SingleVal = new SKColor(0, 0, 0, 0) };

        public StoppedFloat TextHaloWidth { get; internal set; } = new StoppedFloat() { SingleVal = 0 };

        public bool TextIgnorePlacement { get; internal set; } = false;

        public TextJustify TextJustify { get; internal set; } = TextJustify.Center;

        public bool TextKeepUpright { get; internal set; }

        public StoppedFloat TextLetterSpacing { get; internal set; } = new StoppedFloat() { SingleVal = 0 };

        public StoppedFloat TextLineHeight { get; internal set; } = new StoppedFloat() { SingleVal = 1.2f };

        public StoppedFloat TextMaxAngle { get; internal set; } = new StoppedFloat() { SingleVal = 45 };

        public StoppedFloat TextMaxWidth { get; internal set; } = new StoppedFloat() { SingleVal = 10 };

        public Vector TextOffset { get; internal set; } = Vector.Empty;

        public StoppedFloat TextOpacity { get; internal set; } = new StoppedFloat() { SingleVal = 1 };

        public bool TextOptional { get; internal set; } = false;

        public StoppedFloat TextPadding { get; internal set; } = new StoppedFloat() { SingleVal = 2 };

        public MapAlignment TextPitchAlignment { get; internal set; } = MapAlignment.Auto;

        public StoppedFloat TextRadialOffset { get; internal set; } = new StoppedFloat() { SingleVal = 0 };

        public StoppedFloat TextRotate { get; internal set; } = new StoppedFloat() { SingleVal = 0 };

        public MapAlignment TextRotationAlignment { get; internal set; } = MapAlignment.Auto;

        public StoppedFloat TextSize { get; internal set; } = new StoppedFloat() { SingleVal = 16 };

        public TextTransform TextTransform { get; internal set; } = TextTransform.None;

        public Vector TextTranslate { get; internal set; } = Vector.Empty;

        public MapAlignment TextTranslateAnchor { get; internal set; } = MapAlignment.Map;

        public List<MapAlignment> TextVariableAnchor { get; internal set; } = new List<MapAlignment>();

        public List<Orientation> TextWritingMode { get; internal set; } = new List<Orientation>();

        /// <summary>
        /// Create default settings for symbol
        /// </summary>
        public void Update()
        {
            if (TextField == string.Empty)
            {
                return;
            }

            textStyle = new Style();

            var font = TextFont.FirstOrDefault();

            if (font == null)
            {
                return;
            }

            // TODO: Create correct family name
            var familyName = font.ToLower().Contains("condensed") ? font.Substring(0, font.IndexOf(" ", font.IndexOf(" ") + 1)) : font.Substring(0, font.IndexOf(" "));

            textStyle.FontFamily = familyName;
            textStyle.FontWeight = font.ToLower().Contains("medium") ? 500 : font.ToLower().Contains("bold") ? 700 : 400;
            textStyle.FontItalic = font.ToLower().Contains("italic");
        }

        public Symbol CreateIconSymbol(MPoint point, TagsCollection tags, EvaluationContext context)
        {
            if (IconImage == null)
                return null;

            var result = new MGLIconSymbol();

            result.Class = tags.ContainsKey("class") ? tags["class"].ToString() : string.Empty;
            result.Subclass = tags.ContainsKey("subclass") ? tags["subclass"].ToString() : string.Empty;
            result.Rank = tags.ContainsKey("rank") ? int.Parse(tags["rank"].ToString()) : 0;

            var iconName = ReplaceWithTags(context != null ? IconImage.Evaluate(context.Zoom) : IconImage.Evaluate(0), tags, context);

            if (string.IsNullOrEmpty(iconName))
            {
                return null;
            }

            result.Image = spriteAtlas.GetSprite(iconName)?.ToSKImage();

            var width = result.Image == null ? 0 : result.Image.Width;
            var height = result.Image == null ? 0 : result.Image.Height;

            var (anchorX, anchorY) = CalcAnchor(IconAnchor, width, height);

            var offsetX = IconTranslate.X;
            var offsetY = IconTranslate.Y;

            result.Point = point;
            result.Anchor = new MPoint(anchorX, anchorY);
            result.Offset = new MPoint(offsetX, offsetY);
            result.Padding = (float)IconPadding.Evaluate(context);

            result.IconOptional = IconOptional;
            result.IgnorePlacement = IconIgnorePlacement;
            result.IsVisible = IsVisible;

            result.Paint = new MGLPaint("");

            return result;
        }

        public Symbol CreateTextSymbol(MPoint point, TagsCollection tags, EvaluationContext context)
        {
            if (TextField == null)
                return null;

            var textBlock = new TextBlock();

            var result = new MGLTextSymbol(textBlock, textStyle);

            result.Class = tags.ContainsKey("class") ? tags["class"].ToString() : string.Empty;
            result.Subclass = tags.ContainsKey("subclass") ? tags["subclass"].ToString() : string.Empty;
            result.Rank = tags.ContainsKey("rank") ? int.Parse(tags["rank"].ToString()) : 0;

            var fieldName = ReplaceWithTags(TextField, tags, context);
            fieldName = ReplaceWithTransforms(fieldName, TextTransform);

            if (fieldName == string.Empty)
                return null;

            result.Name = fieldName;
            result.TextStyle.FontSize = (float)TextSize.Evaluate(context);

            var typeface = FontMapper.Default.TypefaceFromStyle(result.TextStyle, false);

            result.TextBlock.AddText(result.Name, textStyle);

            result.TextBlock.MaxWidth = (float)TextMaxWidth.Evaluate(context) * result.TextStyle.FontSize;
            result.TextBlock.BaseDirection = TextDirection.Auto;

            var test = result.TextBlock.FontRuns[0];

            switch (TextJustify)
            {
                case TextJustify.Left:
                    result.TextBlock.Alignment = TextAlignment.Left;
                    break;
                case TextJustify.Right:
                    result.TextBlock.Alignment = TextAlignment.Right;
                    break;
                case TextJustify.Center:
                    result.TextBlock.Alignment = TextAlignment.Center;
                    break;
                case TextJustify.Auto:
                default:
                    result.TextBlock.Alignment = TextAlignment.Auto;
                    break;
            }

            var width = (float)result.TextBlock.MeasuredWidth;
            var height = result.TextBlock.MeasuredHeight;

            var (anchorX, anchorY) = CalcAnchor(TextAnchor, width, height);

            // If TextVariableAnchors has a value, TextOffset are absolut values, otherwise TextOffset in ems
            var offsetX = TextOffset.X * (TextVariableAnchor.Count > 0 ? 1 : result.TextStyle.FontSize);
            var offsetY = TextOffset.Y * (TextVariableAnchor.Count > 0 ? 1 : result.TextStyle.FontSize);

            result.Point = point;
            result.Anchor = new MPoint(anchorX, anchorY);
            result.Offset = new MPoint(offsetX, offsetY);
            result.Padding = (float)TextPadding.Evaluate(context);

            result.TextOptional = TextOptional;
            result.IsVisible = IsVisible;

            var paint = new MGLPaint(result.Name);

            if (TextColor.Stops != null)
            {
                paint.SetVariableColor((context) => TextColor.Evaluate(context.Zoom));
            }
            else
            {
                paint.SetFixColor((SKColor)TextColor.SingleVal);
            }

            if (TextOpacity.Stops != null)
            {
                paint.SetVariableOpacity((context) => TextOpacity.Evaluate(context.Zoom));
            }
            else
            {
                paint.SetFixOpacity(TextOpacity.SingleVal);
            }

            result.Paint = paint;

            //if (result.Name == "Chapiteau de Fontvieille")
            //    System.Diagnostics.Debug.WriteLine($"Envelope for {result.Name}: {result.Envelope.MinX}, {result.Envelope.MinY}, {result.Envelope.MaxX}, {result.Envelope.MaxY}");

            return result;
        }

        public Symbol CreateIconTextSymbol(MPoint point, TagsCollection tags, EvaluationContext context)
        {
            var icon = (MGLIconSymbol)CreateIconSymbol(point, tags, context);
            var text = (MGLTextSymbol)CreateTextSymbol(point, tags, context);

            return new MGLIconTextSymbol(icon, text);
        }

        public Symbol CreatePathSymbols(VectorElement element, EvaluationContext context)
        {
            var result = new MGLPathSymbol(element.TileIndex, element.Id);

            result.Class = element.Tags.ContainsKey("class") ? element.Tags["class"].ToString() : string.Empty;
            result.Subclass = element.Tags.ContainsKey("subclass") ? element.Tags["subclass"].ToString() : string.Empty;
            result.Rank = element.Tags.ContainsKey("rank") ? int.Parse(element.Tags["rank"].ToString()) : 0;

            result.Name = ReplaceWithTags(TextField, element.Tags, context);

            return result;
        }

        private string ReplaceWithTags(string text, TagsCollection tags, EvaluationContext context = null)
        {
            var match = regex.Match(text);

            if (!match.Success)
                return text;

            var val = match.Groups[1].Value;

            if (tags.ContainsKey(val))
                return text.Replace($"{{{val}}}", (string)tags[val]);

            if (context != null && context.Tags != null && context.Tags.ContainsKey(val))
                return text.Replace($"{{{val}}}", (string)context.Tags[val]);

            // Check, if match starts with name
            if (val.StartsWith("name"))
            {
                // Try to take the localized name
                var code = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                if (tags.ContainsKey("name:"+code))
                    return text.Replace($"{{{val}}}", (string)tags["name:" + code]);
                if (tags.ContainsKey("name_" + code))
                    return text.Replace($"{{{val}}}", (string)tags["name_" + code]);
            }

            return text;
        }

        private string ReplaceWithTransforms(string text, TextTransform textTransform)
        {
            switch (textTransform)
            {
                case TextTransform.Uppercase:
                    return text.ToUpper();
                case TextTransform.Lowercase:
                    return text.ToLower();
            }

            return text;
        }

        private (float anchorX, float anchorY) CalcAnchor(Direction direction, float width, float height)
        {
            var anchorX = 0f;
            var anchorY = 0f;

            switch (direction)
            {
                case Direction.Top:
                case Direction.Center:
                case Direction.Bottom:
                    anchorX = -width / 2;
                    break;
                case Direction.Right:
                case Direction.BottomRight:
                case Direction.TopRight:
                    anchorX = -width;
                    break;
            }

            switch (direction)
            {
                case Direction.Center:
                case Direction.Right:
                case Direction.Left:
                    anchorY = -height / 2;
                    break;
                case Direction.Bottom:
                case Direction.BottomRight:
                case Direction.BottomLeft:
                    anchorY = -height;
                    break;
            }

            return (anchorX, anchorY);
        }
    }
}
