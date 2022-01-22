using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Mapsui.VectorTileLayers.OpenMapTiles.Converter;
using Mapsui.VectorTileLayers.OpenMapTiles.Expressions;

namespace Mapsui.VectorTileLayers.OpenMapTiles.Json
{
    /// <summary>
    /// Class holding Layout data in Json format
    /// </summary>
    public class JsonLayout
    {
        [JsonProperty("line-cap")]
        public string LineCap { get; set; } = "butt";

        [JsonProperty("line-join")]
        public string LineJoin { get; set; } = "miter";

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("line-miter-limit")]
        public StoppedFloat LineMiterLimit { get; set; } = new StoppedFloat { SingleVal = 2f };

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("line-round-limit")]
        public StoppedFloat LineRoundLimit { get; set; } = new StoppedFloat { SingleVal = 1.05f };

        [JsonProperty("visibility")]
        public string Visibility { get; set; } = "visible";

        [JsonProperty("text-font")]
        public JArray TextFont { get; set; }

        [JsonProperty("text-field")]
        public string TextField { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("text-max-width")]
        public StoppedFloat TextMaxWidth { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("text-size")]
        public StoppedFloat TextSize { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("text-padding")]
        public StoppedFloat TextPadding { get; set; }

        [JsonProperty("text-offset")]
        public float[] TextOffset { get; set; }

        [JsonProperty("text-optional")]
        public bool TextOptional { get; set; }

        [JsonProperty("text-allow-overlap")]
        public bool TextAllowOverlap { get; set; }

        [JsonProperty("text-anchor")]
        public string TextAnchor { get; set; }

        [JsonProperty("text-justify")]
        public string TextJustify { get; set; }

        [JsonProperty("text-rotation-alignment")]
        public string TextRotationAlignment { get; set; }

        [JsonProperty("icon-rotation-alignment")]
        public string IconRotationAlignment { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("text-max-angle")]
        public StoppedFloat TextMaxAngle { get; set; }

        [JsonProperty("text-transform")]
        public string TextTransform { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("text-letter-spacing")]
        public StoppedFloat TextLetterSpacing { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("text-line-height")]
        public StoppedFloat TextLineHeight { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("text-halo-blur")]
        public StoppedFloat TextHaloBlur { get; set; }

        [JsonConverter(typeof(StoppedColorConverter))]
        [JsonProperty("text-halo-color")]
        public StoppedColor TextHaloColor { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("text-halo-width")]
        public StoppedFloat TextHaloWidth { get; set; }

        [JsonProperty("text-ignore-placement")]
        public bool TextIgnorePlacement { get; set; }

        [JsonProperty("text-keep-upright")]
        public bool TextKeepUpright { get; set; }

        [JsonProperty("text-pitch-alignment")]
        public string TextPitchAlignment { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("text-radial-offset")]
        public StoppedFloat TextRadialOffset { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("text-rotate")]
        public StoppedFloat TextRotate { get; set; }

        [JsonProperty("text-translate")]
        public float[] TextTranslate { get; set; }

        [JsonProperty("text-translate-anchor")]
        public string TextTranslateAnchor { get; set; }

        [JsonProperty("text-variable-anchor")]
        public string[] TextVariableAnchor { get; set; }

        [JsonProperty("text-writing-mode")]
        public string[] TextWritingMode { get; set; }

        [JsonConverter(typeof(StoppedStringConverter))]
        [JsonProperty("icon-image")]
        public StoppedString IconImage { get; set; }

        [JsonConverter(typeof(StoppedStringConverter))]
        [JsonProperty("symbol-placement")]
        public StoppedString SymbolPlacement { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("symbol-spacing")]
        public StoppedFloat SymbolSpacing { get; set; }

        [JsonProperty("symbol-z-order")]
        public string SymbolZOrder { get; set; }

        [JsonProperty("icon-anchor")]
        public string IconAnchor { get; set; } = "center";

        [JsonConverter(typeof(StoppedColorConverter))]
        [JsonProperty("icon-color")]
        public StoppedColor IconColor { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("icon-halo-blur")]
        public StoppedFloat IconHaloBlur { get; set; }

        [JsonConverter(typeof(StoppedColorConverter))]
        [JsonProperty("icon-halo-color")]
        public StoppedColor IconHaloColor { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("icon-halo-width")]
        public StoppedFloat IconHaloWidth { get; set; }

        [JsonProperty("icon-ignore-placement")]
        public bool IconIgnorePlacement { get; set; } = false;

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("icon-padding")]
        public StoppedFloat IconPadding { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("icon-size")]
        public StoppedFloat IconSize { get; set; }

        [JsonProperty("icon-offset")]
        public float[] IconOffset { get; set; }

        [JsonProperty("icon-optional")]
        public bool IconOptional { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("icon-opacity")]
        public StoppedFloat IconOpacity { get; set; }

        [JsonProperty("icon-allow-overlap")]
        public bool IconAllowOverlap { get; set; } = false;

        [JsonProperty("icon-keep-upright")]
        public bool IconKeepUpright { get; set; } = false;

        [JsonProperty("icon-pitch-alignment")]
        public string IconPitchAlignment { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("icon-rotate")]
        public StoppedFloat IconRotate { get; set; }

        [JsonProperty("icon-text-fit")]
        public string IconTextFit { get; set; }

        [JsonProperty("icon-text-fit-padding")]
        public float[] IconTextFitPadding { get; set; }

        [JsonProperty("icon-translate")]
        public float[] IconTranslate { get; set; }

        [JsonProperty("icon-translate-anchor")]
        public string IconTranslateAnchor { get; set; }

        [JsonProperty("symbol-avoid-edges")]
        public bool SymbolAvoidEdges { get; set; }

        [JsonProperty("symbol-sork-key")]
        public float SymbolSortKey { get; set; }

    }
}
