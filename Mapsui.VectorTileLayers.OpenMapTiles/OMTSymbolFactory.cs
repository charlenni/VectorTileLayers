using Mapsui.VectorTileLayers.Core.Enums;
using Mapsui.VectorTileLayers.Core.Interfaces;
using Mapsui.VectorTileLayers.Core.Primitives;
using Mapsui.VectorTileLayer.Mapbox.Extensions;
using Mapsui.VectorTileLayers.OpenMapTiles.Expressions;
using SkiaSharp;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Topten.RichTextKit;
using Mapsui.VectorTileLayers.Core.Extensions;
using System;

namespace Mapsui.VectorTileLayers.OpenMapTiles
{
    public class OMTSymbolFactory : IVectorSymbolFactory
    {
        static Regex regex = new Regex(@".*\{(.*)\}.*");

        public static Dictionary<string, SKTypeface> SpecialFonts = new Dictionary<string, SKTypeface>();

        OMTSpriteAtlas spriteAtlas;
        Style textStyle;

        public static OMTSymbolFactory Default;

        public OMTSymbolFactory(OMTSpriteAtlas atlas)
        {
            spriteAtlas = atlas;
        }

        public bool HasIcon { get => IconImage != null; }

        public bool HasText { get => TextField != null && TextFont != null; }

        public bool IsVisible { get; internal set; } = true;

        public bool IconAllowOverlap { get; internal set; }

        public Direction IconAnchor { get; internal set; } = Direction.Center;

        public StoppedColor IconColor { get; internal set; } = new StoppedColor() { SingleVal = new SKColor(0, 0, 0, 255) };

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

        public StoppedColor TextColor { get; internal set; } = new StoppedColor() { SingleVal = new SKColor(0, 0, 0, 255) };

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
        }

        private Style CreateTextStyle()
        {
            var font = TextFont.FirstOrDefault();
            var textStyle = new Style();

            if (font == null)
            {
                return textStyle;
            }

            // TODO: Create correct family name
            var fontFamilyName = font;

            if (fontFamilyName.Contains("condensed", System.StringComparison.InvariantCultureIgnoreCase))
            {
                textStyle.FontWidth = SKFontStyleWidth.Condensed;
                fontFamilyName = fontFamilyName.Replace("condensed", "", System.StringComparison.InvariantCultureIgnoreCase);
            }

            textStyle.FontWeight = 400;

            if (fontFamilyName.Contains("regular", System.StringComparison.InvariantCultureIgnoreCase))
            {
                textStyle.FontWeight = 400;
                fontFamilyName = fontFamilyName.Replace("regular", "", System.StringComparison.InvariantCultureIgnoreCase);
            }

            if (fontFamilyName.Contains("medium", System.StringComparison.InvariantCultureIgnoreCase))
            {
                textStyle.FontWeight = 500;
                fontFamilyName = fontFamilyName.Replace("medium", "", System.StringComparison.InvariantCultureIgnoreCase);
            }

            if (fontFamilyName.Contains("bold", System.StringComparison.InvariantCultureIgnoreCase))
            {
                textStyle.FontWeight = 500;
                fontFamilyName = fontFamilyName.Replace("bold", "", System.StringComparison.InvariantCultureIgnoreCase);
            }

            if (fontFamilyName.Contains("italic", System.StringComparison.InvariantCultureIgnoreCase))
            {
                textStyle.FontItalic = true;
                fontFamilyName = fontFamilyName.Replace("italic", "", System.StringComparison.InvariantCultureIgnoreCase);
            }

            fontFamilyName = fontFamilyName.Replace("  ", " ").Trim();

            textStyle.FontFamily = fontFamilyName;

            return textStyle;
        }

        public Symbol CreateIconSymbol(MPoint point, TagsCollection tags, EvaluationContext context)
        {
            if (IconImage == null)
                return null;

            var result = new OMTIconSymbol();

            // Set orientation
            result.Alignment = GetAlignment(IconPitchAlignment, IconRotationAlignment, ((string)SymbolPlacement.Evaluate(context)).ToLower());

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

            result.IconSize = IconSize.Evaluate(context.Zoom);
            result.IconOptional = IconOptional;
            result.IgnorePlacement = IconIgnorePlacement;
            result.IsVisible = IsVisible;

            result.Paint = new OMTPaint("");

            return result;
        }

        public Symbol CreateTextSymbol(MPoint point, TagsCollection tags, EvaluationContext context)
        {
            if (TextField == null)
                return null;

            var textBlock = new TextBlock();

            textStyle = CreateTextStyle();

            var result = new OMTTextSymbol(textBlock, textStyle);

            // Set orientation
            result.Alignment = GetAlignment(TextPitchAlignment, TextRotationAlignment, ((string)SymbolPlacement.Evaluate(context)).ToLower());

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
            result.TextHaloBlur = TextHaloBlur;
            result.TextHaloColor = TextHaloColor;
            result.TextHaloWidth = TextHaloWidth;
            result.IsVisible = IsVisible;

            var paint = new OMTPaint(result.Name);

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
            var icon = (OMTIconSymbol)CreateIconSymbol(point, tags, context);
            var text = (OMTTextSymbol)CreateTextSymbol(point, tags, context);

            return new OMTIconTextSymbol(icon, text);
        }

        public IEnumerable<Symbol> CreatePathSymbols(VectorElement element, EvaluationContext context)
        {
            var result = new List<Symbol>();
            //var symbol = new OMTPathSymbol(element.TileIndex, element.Id);

            //symbol.Class = element.Tags.ContainsKey("class") ? element.Tags["class"].ToString() : string.Empty;
            //symbol.Subclass = element.Tags.ContainsKey("subclass") ? element.Tags["subclass"].ToString() : string.Empty;
            //symbol.Rank = element.Tags.ContainsKey("rank") ? int.Parse(element.Tags["rank"].ToString()) : 0;

            //symbol.Name = ReplaceWithTags(TextField, element.Tags, context);
            //symbol.Name = ReplaceWithTransforms(result.Name, TextTransform);

            //if (symbol.Name == string.Empty)
            //    return null;

            if (((string)SymbolPlacement.Evaluate(context)).ToLower() == "point" && element.IsPoint)
            { }

            if (((string)SymbolPlacement.Evaluate(context)).ToLower() == "line-center" && (element.IsLine || element.IsPolygon))
            { }

            if (((string)SymbolPlacement.Evaluate(context)).ToLower() == "line" && (element.IsLine || element.IsPolygon))
            {
                var path = new SKPath();
                element.AddToPath(path);
                var spacing = (float)SymbolSpacing.Evaluate(context);
                var pos = 0.5f;
                using (var pathMeasure = new SKPathMeasure(path))
                {
                    while (pathMeasure.Length > spacing * pos)
                    {
                        pathMeasure.GetPositionAndTangent(spacing * pos, out var position, out var tangent);
                        try
                        {
                            var symbol = CreateIconTextSymbol(position.ToPoint(), element.Tags, context);
                            symbol.Index = element.TileIndex;
                            result.Add(symbol);
                        }
                        catch (Exception e)
                        { }
                        pos++;
                    }
                }
            }
            else if (((string)SymbolPlacement.Evaluate(context)).ToLower() == "point")
            {

            }

            return result;
        }

        private MapAlignment GetAlignment(MapAlignment pitchAlignment, MapAlignment rotationAlignment, string symbolPlacement)
        {
            MapAlignment result = MapAlignment.Map;

            switch (pitchAlignment)
            {
                case MapAlignment.Map:
                    result = MapAlignment.Map;
                    break;
                case MapAlignment.Viewport:
                    result = MapAlignment.Viewport;
                    break;
                case MapAlignment.Auto:
                    switch (rotationAlignment)
                    {
                        case MapAlignment.Map:
                            result = MapAlignment.Map;
                            break;
                        case MapAlignment.Viewport:
                            result = MapAlignment.Viewport;
                            break;
                        case MapAlignment.Auto:
                            if (symbolPlacement == "point")
                                result = MapAlignment.Viewport;
                            break;
                    }
                    break;
            }

            return result;
        }

        private string ReplaceWithTags(string text, TagsCollection tags, EvaluationContext context = null)
        {
            var match = regex.Match(text);

            if (!match.Success)
                return text;

            var val = match.Groups[1].Value;

            if (tags.ContainsKey(val))
                return text.Replace($"{{{val}}}", tags[val].ToString());

            if (context != null && context.Tags != null && context.Tags.ContainsKey(val))
                return text.Replace($"{{{val}}}", context.Tags[val].ToString());

            // Check, if match starts with name
            if (val.StartsWith("name"))
            {
                // Try to take the localized name
                var code = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                if (tags.ContainsKey("name:"+code))
                    return text.Replace($"{{{val}}}", tags["name:" + code].ToString());
                if (tags.ContainsKey("name_" + code))
                    return text.Replace($"{{{val}}}", tags["name_" + code].ToString());
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
