using Newtonsoft.Json;
using System.Collections.Generic;
using Mapsui.VectorTileLayer.MapboxGL.Converter;
using Mapsui.VectorTileLayer.MapboxGL.Expressions;

namespace Mapsui.VectorTileLayer.MapboxGL.Json
{
    public class JsonPaint
    {
        [JsonConverter(typeof(StoppedColorConverter))]
        [JsonProperty("background-color")]
        public StoppedColor BackgroundColor { get; set; }

        [JsonConverter(typeof(StoppedStringConverter))]
        [JsonProperty("background-pattern")]
        public StoppedString BackgroundPattern { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("background-opacity")]
        public StoppedFloat BackgroundOpacity { get; set; }

        [JsonConverter(typeof(StoppedColorConverter))]
        [JsonProperty("fill-color")]
        public StoppedColor FillColor { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("fill-opacity")]
        public StoppedFloat FillOpacity { get; set; }

        [JsonConverter(typeof(StoppedColorConverter))]
        [JsonProperty("line-color")]
        public StoppedColor LineColor { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("line-width")]
        public StoppedFloat LineWidth { get; set; }

        [JsonProperty("fill-translate")]
        public object FillTranslate { get; set; }

        [JsonConverter(typeof(StoppedStringConverter))]
        [JsonProperty("fill-pattern")]
        public StoppedString FillPattern { get; set; }

        [JsonConverter(typeof(StoppedColorConverter))]
        [JsonProperty("fill-outline-color")]
        public StoppedColor FillOutlineColor { get; set; }

        [JsonConverter(typeof(StoppedFloatArrayConverter))]
        [JsonProperty("line-dasharray")]
        public StoppedFloatArray LineDashArray { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("line-opacity")]
        public StoppedFloat LineOpacity { get; set; }

        [JsonConverter(typeof(StoppedColorConverter))]
        [JsonProperty("text-color")]
        public StoppedColor TextColor { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("text-halo-width")]
        public StoppedFloat TextHaloWidth { get; set; }

        [JsonConverter(typeof(StoppedColorConverter))]
        [JsonProperty("text-halo-color")]
        public StoppedColor TextHaloColor { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("text-halo-blur")]
        public StoppedFloat TextHaloBlur { get; set; }

        [JsonConverter(typeof(StoppedBooleanConverter))]
        [JsonProperty("fill-antialias")]
        public StoppedBoolean FillAntialias { get; set; }

        [JsonProperty("fill-translate-anchor")]
        public string FillTranslateAnchor { get; set; }

        [JsonProperty("line-gap-width")]
        public object LineGapWidth { get; set; }

        [JsonProperty("line-blur")]
        public object LineBlur { get; set; }

        [JsonProperty("line-translate")]
        public IList<int> LineTranslate { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("icon-halo-blur")]
        public StoppedFloat IconHaloBlur { get; set; }

        [JsonConverter(typeof(StoppedColorConverter))]
        [JsonProperty("icon-halo-color")]
        public StoppedColor IconHaloColor { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("icon-halo-width")]
        public StoppedFloat IconHaloWidth { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("text-opacity")]
        public StoppedFloat TextOpacity { get; set; }

        [JsonConverter(typeof(StoppedColorConverter))]
        [JsonProperty("icon-color")]
        public StoppedColor IconColor { get; set; }

        [JsonProperty("text-translate")]
        public IList<int> TextTranslate { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("icon-opacity")]
        public StoppedFloat IconOpacity { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("raster-opacity")]
        public StoppedFloat RasterOpacity { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("raster-hue-rotate")]
        public StoppedFloat RasterHueRotate { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("raster-brightness-min")]
        public StoppedFloat RasterBrightnessMin { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("raster-brightness-max")]
        public StoppedFloat RasterBrightnessMax { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("raster-saturation")]
        public StoppedFloat RasterSaturation { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("raster-contrast")]
        public StoppedFloat RasterContrast { get; set; }

        [JsonConverter(typeof(StoppedFloatConverter))]
        [JsonProperty("raster-fade-duration")]
        public StoppedFloat RasterFadeDuration { get; set; }
    }
}
