using Mapsui.VectorTileLayer.OpenMapTiles.Extensions;
using Mapsui.VectorTileLayers.Core.Enums;
using Mapsui.VectorTileLayers.Core.Extensions;
using Mapsui.VectorTileLayers.Core.Interfaces;
using Mapsui.VectorTileLayers.Core.Primitives;
using Mapsui.VectorTileLayers.OpenMapTiles.Expressions;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Topten.RichTextKit;

namespace Mapsui.VectorTileLayers.OpenMapTiles
{
    /// <summary>
    /// OMTSymbolFactory creates all symbols of one StyleLayer
    /// </summary>
    /// <remarks>
    /// This symbols could belong to different contexts/zoom levels, 
    /// but each symbol has only settings for one zoom level.
    /// </remarks>
    public class OMTSymbolFactory : IVectorSymbolFactory
    {
        static Regex regex = new Regex(@".*\{(.*)\}.*");

        public static Dictionary<string, SKTypeface> SpecialFonts = new Dictionary<string, SKTypeface>();

        OMTSpriteAtlas spriteAtlas;
        Style textStyle;

        public static OMTSymbolFactory Default;

        public OMTSymbolFactory(string styleLayerName, OMTSpriteAtlas atlas)
        {
            StyleLayerName = styleLayerName;
            spriteAtlas = atlas;
        }

        public string StyleLayerName { get; }

        public bool HasIcon { get => IconImage != null; }

        public bool HasText { get => TextField != null && TextFont != null; }

        public bool IsVisible { get; internal set; } = true;

        #region Common symbol settings

        public bool SymbolAvoidEdges { get; internal set; }

        public StoppedString SymbolPlacement { get; internal set; }

        public float SymbolSortKey { get; internal set; }

        public StoppedFloat SymbolSpacing { get; internal set; }

        public ZOrder SymbolZOrder { get; internal set; } = ZOrder.Auto;

        #endregion

        #region Icon Layout settings

        public bool IconAllowOverlap { get; internal set; }

        public Direction IconAnchor { get; internal set; } = Direction.Center;

        public bool IconIgnorePlacement { get; internal set; } = false;

        public StoppedString IconImage { get; internal set; }

        public bool IconKeepUpright { get; internal set; }

        public Vector IconOffset { get; internal set; } = Vector.Empty;

        public bool IconOptional { get; internal set; } = false;

        public StoppedFloat IconPadding { get; internal set; } = new StoppedFloat() { SingleVal = 2 };

        public MapAlignment IconPitchAlignment { get; internal set; } = MapAlignment.Auto;

        public StoppedFloat IconRotate { get; internal set; } = new StoppedFloat() { SingleVal = 0 };

        public MapAlignment IconRotationAlignment { get; internal set; } = MapAlignment.Auto;

        public StoppedFloat IconSize { get; internal set; } = new StoppedFloat() { SingleVal = 1 };

        public TextFit IconTextFit { get; internal set; } = TextFit.None;

        public MRect IconTextFitPadding { get; internal set; } = new MRect(0, 0, 0, 0);

        #endregion

        #region Icon Paint settings

        public StoppedColor IconColor { get; internal set; } = new StoppedColor() { SingleVal = new SKColor(0, 0, 0, 255) };

        public StoppedFloat IconHaloBlur { get; internal set; } = new StoppedFloat() { SingleVal = 0 };

        public StoppedColor IconHaloColor { get; internal set; } = new StoppedColor() { SingleVal = new SKColor(0, 0, 0, 0) };

        public StoppedFloat IconHaloWidth { get; internal set; } = new StoppedFloat() { SingleVal = 0 };

        public StoppedFloat IconOpacity { get; internal set; } = new StoppedFloat() { SingleVal = 1 };

        public Vector IconTranslate { get; internal set; } = Vector.Empty;

        public MapAlignment IconTranslateAnchor { get; internal set; } = MapAlignment.Map;

        #endregion

        #region Text Layout settings

        public bool TextAllowOverlap { get; internal set; }

        public Direction TextAnchor { get; internal set; } = Direction.Center;

        public string TextField { get; internal set; } = "";

        public List<string> TextFont { get; internal set; } = new List<string>();

        public bool TextIgnorePlacement { get; internal set; } = false;

        public TextJustify TextJustify { get; internal set; } = TextJustify.Center;

        public bool TextKeepUpright { get; internal set; }

        public StoppedFloat TextLetterSpacing { get; internal set; } = new StoppedFloat() { SingleVal = 0 };

        public StoppedFloat TextLineHeight { get; internal set; } = new StoppedFloat() { SingleVal = 1.2f };

        public StoppedFloat TextMaxAngle { get; internal set; } = new StoppedFloat() { SingleVal = 45 };

        public StoppedFloat TextMaxWidth { get; internal set; } = new StoppedFloat() { SingleVal = 10 };

        public Vector TextOffset { get; internal set; } = Vector.Empty;

        public bool TextOptional { get; internal set; } = false;

        public StoppedFloat TextPadding { get; internal set; } = new StoppedFloat() { SingleVal = 2 };

        public MapAlignment TextPitchAlignment { get; internal set; } = MapAlignment.Auto;

        public StoppedFloat TextRadialOffset { get; internal set; } = new StoppedFloat() { SingleVal = 0 };

        public StoppedFloat TextRotate { get; internal set; } = new StoppedFloat() { SingleVal = 0 };

        public MapAlignment TextRotationAlignment { get; internal set; } = MapAlignment.Auto;

        public StoppedFloat TextSize { get; internal set; } = new StoppedFloat() { SingleVal = 16 };

        public TextTransform TextTransform { get; internal set; } = TextTransform.None;

        public List<Direction> TextVariableAnchor { get; internal set; } = new List<Direction>();

        public List<Orientation> TextWritingMode { get; internal set; } = new List<Orientation>();

        #endregion

        #region Text Paint settings

        public StoppedColor TextColor { get; internal set; } = new StoppedColor() { SingleVal = new SKColor(0, 0, 0, 255) };

        public StoppedFloat TextHaloBlur { get; internal set; } = new StoppedFloat() { SingleVal = 0 };

        public StoppedColor TextHaloColor { get; internal set; } = new StoppedColor() { SingleVal = new SKColor(0, 0, 0, 0) };

        public StoppedFloat TextHaloWidth { get; internal set; } = new StoppedFloat() { SingleVal = 0 };

        public StoppedFloat TextOpacity { get; internal set; } = new StoppedFloat() { SingleVal = 1 };

        public Vector TextTranslate { get; internal set; } = Vector.Empty;

        public MapAlignment TextTranslateAnchor { get; internal set; } = MapAlignment.Map;

        #endregion

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

        public Symbol CreateTextSymbol(MPoint point, TagsCollection tags, EvaluationContext context)
        {
            // Is there something to make?
            if (string.IsNullOrEmpty(TextField))
                return null;

            var fieldName = ReplaceWithTags(TextField, tags, context);
            fieldName = ReplaceWithTransforms(fieldName, TextTransform);

            if (string.IsNullOrEmpty(fieldName))
                return null;

            // Now we are sure, that the symbol contains something, that should be shown
            var textBlock = new TextBlock();

            textStyle = CreateTextStyle();

            var result = new OMTTextSymbol(textBlock, textStyle);

            // Set orientation
            result.Alignment = GetAlignment(TextPitchAlignment, TextRotationAlignment, ((string)SymbolPlacement.Evaluate(context)).ToLower());

            result.Class = tags.ContainsKey("class") ? tags["class"].ToString() : string.Empty;
            result.Subclass = tags.ContainsKey("subclass") ? tags["subclass"].ToString() : string.Empty;
            result.Rank = tags.ContainsKey("rank") ? int.Parse(tags["rank"].ToString()) : 0;

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
            var offsetX = TextOffset.X * result.TextStyle.FontSize;  // TODO: Is FontSize == ems size?
            var offsetY = TextOffset.Y * result.TextStyle.FontSize;

            result.Point = point;
            result.PossibleAnchors = GetAllAnchors(TextAnchor, TextVariableAnchor, width, height);
            result.Anchor = result.PossibleAnchors.FirstOrDefault();
            result.AnchorType = TextVariableAnchor.Count > 0 ? AnchorType.Variable : AnchorType.Fixed;
            result.Offset = TextVariableAnchor.Count > 0 ? new MPoint(Math.Abs(offsetX), Math.Abs(offsetY)) : new MPoint(offsetX, offsetY);
            result.Padding = (float)TextPadding.Evaluate(context);

            // Add Text Paint settings for later calculation
            result.TextColor = TextColor;
            result.TextOpacity = TextOpacity;
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

        public Symbol CreateIconSymbol(MPoint point, float rotation, TagsCollection tags, EvaluationContext context)
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

            var size = (float)IconSize.Evaluate(context);

            var width = result.Image == null ? 0 : result.Image.Width * size;
            var height = result.Image == null ? 0 : result.Image.Height * size;

            var offsetX = IconTranslate.X + IconOffset.X * (float)IconSize.Evaluate(context);
            var offsetY = IconTranslate.Y + IconOffset.Y * (float)IconSize.Evaluate(context);

            result.Point = point;
            result.Translate = new MPoint(IconTranslate.X, IconTranslate.Y);
            result.TranslateAnchor = IconTranslateAnchor;
            result.Anchor = GetAllAnchors(IconAnchor, null, width, height).FirstOrDefault();
            result.Offset = new MPoint(offsetX, offsetY);
            result.Padding = (float)IconPadding.Evaluate(context);

            result.IconSize = IconSize.Evaluate(context.Zoom);
            result.Rotation = rotation == 0 ? (float)IconRotate.Evaluate(context) : rotation;
            result.IconOptional = IconOptional;
            result.IgnorePlacement = IconIgnorePlacement;
            result.IsVisible = IsVisible;

            result.Paint = new OMTPaint("");

            return result;
        }

        public Symbol CreateIconTextSymbol(MPoint point, float rotation, TagsCollection tags, EvaluationContext context)
        {
            var icon = (OMTIconSymbol)CreateIconSymbol(point, rotation, tags, context);
            var text = (OMTTextSymbol)CreateTextSymbol(point, tags, context);

            return new OMTIconTextSymbol(icon, text);
        }

        public IEnumerable<Symbol> CreatePathSymbols(VectorElement element, EvaluationContext context)
        {
            List<Symbol> result = new List<Symbol>();
            
            //var symbol = new OMTPathSymbol(element.TileIndex, element.Id);

            //symbol.Class = element.Tags.ContainsKey("class") ? element.Tags["class"].ToString() : string.Empty;
            //symbol.Subclass = element.Tags.ContainsKey("subclass") ? element.Tags["subclass"].ToString() : string.Empty;
            //symbol.Rank = element.Tags.ContainsKey("rank") ? int.Parse(element.Tags["rank"].ToString()) : 0;

            //symbol.Name = ReplaceWithTags(TextField, element.Tags, context);
            //symbol.Name = ReplaceWithTransforms(result.Name, TextTransform);

            //if (symbol.Name == string.Empty)
            //    return null;

            if (((string)SymbolPlacement.Evaluate(context)).Equals("point", StringComparison.InvariantCultureIgnoreCase) && element.IsPoint)
            { }

            if (((string)SymbolPlacement.Evaluate(context)).Equals("line-center", StringComparison.InvariantCultureIgnoreCase) && (element.IsLine || element.IsPolygon))
            { }

            if (((string)SymbolPlacement.Evaluate(context)).Equals("line", StringComparison.InvariantCultureIgnoreCase) && (element.IsLine || element.IsPolygon))
            {
                // Evaluate context
                string iconName = IconImage != null ? ReplaceWithTags((string)(IconImage.Evaluate(context)), element.Tags, context) : "";
                string textName = !string.IsNullOrEmpty(TextField) ? ReplaceWithTransforms(ReplaceWithTags(TextField, element.Tags, context), TextTransform) : "";
                
                // Is there something to do?
                if (string.IsNullOrEmpty(iconName) && string.IsNullOrEmpty(textName))
                    return null;

                float length = 0;

                // Get size of icon and text for this context
                if (!string.IsNullOrEmpty(iconName))
                {
                    var sprite = spriteAtlas.GetSprite(iconName);
                    // TODO: Enable when sprite problem is solved
                    //if (sprite != null)
                    //    length = sprite.ToSKImage().Width + (IconPadding != null ? (float)(IconPadding.Evaluate(context)) : 0);
                }

                // Calc positions
                var path = new SKPath();
                element.AddToPath(path);
                var spacing = (float)SymbolSpacing.Evaluate(context);
                using (var pathMeasure = new SKPathMeasure(path))
                {
                    if (pathMeasure.Length < length)
                        return null;
                    if (pathMeasure.Length < spacing)
                        spacing = pathMeasure.Length / 2;
                    // Calculate a start distance from the first point
                    var nextPosition = spacing; // pathMeasure.Length > spacing * 0.5f ? 0.5f : 0.1f;
                    while (pathMeasure.Length > nextPosition)
                    {
                        pathMeasure.GetPositionAndTangent(nextPosition, out var position, out var tangentVec);
                        try
                        {
                            var tangent = 360f - (float)(Math.Atan2(tangentVec.Y, tangentVec.X) * 180 / Math.PI);
                            var rotation = -(float)IconRotate.Evaluate(context);
                            rotation -= (IconRotationAlignment == MapAlignment.Map || IconRotationAlignment == MapAlignment.Auto ? tangent : 0f);
                            rotation %= 360;
                            var symbol = CreateIconTextSymbol(position.ToPoint(), rotation, element.Tags, context);
                            symbol.Index = element.TileIndex;
                            result.Add(symbol);
                        }
                        catch
                        { }
                        nextPosition += spacing;
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
                            if (symbolPlacement.Equals("point", StringComparison.InvariantCultureIgnoreCase))
                                result = MapAlignment.Map;
                            // In other cases it is a path alignment
                            break;
                        case MapAlignment.Viewport:
                            result = MapAlignment.Viewport;
                            break;
                        case MapAlignment.Auto:
                            if (symbolPlacement.Equals("point", StringComparison.InvariantCultureIgnoreCase))
                                result = MapAlignment.Viewport;
                            // Other cases it is MapAlignment.Map
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

                // We didn't find a name in the tags, so remove this part
                return text.Replace($"{{{val}}}", "");
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

        private List<MPoint> GetAllAnchors(Direction fixedAnchor, List<Direction> variableAnchor, float width, float height)
        {
            // We have no variable anchor, so use the fixed anchor
            if (variableAnchor == null || variableAnchor.Count == 0)
            {
                return new List<MPoint> { CalcAnchor(fixedAnchor, width, height) };
            }

            var result = new List<MPoint>(variableAnchor.Count);

            foreach (var direction in variableAnchor)
            {
                result.Add(CalcAnchor(direction, width, height));
            }

            return result;
        }

        private MPoint CalcAnchor(Direction direction, float width, float height)
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

            return new MPoint(anchorX, anchorY);
        }
    }
}
