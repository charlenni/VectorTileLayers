using Mapsui.VectorTileLayers.Core.Primitives;
using Mapsui.VectorTileLayer.Mapbox.Extensions;
using Mapsui.VectorTileLayers.OpenMapTiles.Extensions;
using Mapsui.VectorTileLayers.OpenMapTiles.Json;
using SkiaSharp;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Mapsui.VectorTileLayers.OpenMapTiles.Converter
{
    public static class StyleLayerConverter
    {
        public static List<OMTPaint> ConvertBackgroundLayer(JsonStyleLayer jsonStyleLayer, OMTSpriteAtlas spriteAtlas)
        {
            var paint = jsonStyleLayer.Paint;

            var brush = new OMTPaint(jsonStyleLayer.Id);

            brush.SetFixStyle(SKPaintStyle.Fill);
            brush.SetFixColor(new SKColor(0, 0, 0, 255));
            brush.SetFixOpacity(1);

            // background-color
            //   Optional color. Defaults to #000000. Disabled by background-pattern. Transitionable.
            //   The color with which the background will be drawn.
            if (paint.BackgroundColor != null)
            {
                if (paint.BackgroundColor.Stops != null)
                {
                    brush.SetVariableColor((context) => paint.BackgroundColor.Evaluate(context.Zoom));
                }
                else
                {
                    brush.SetFixColor((SKColor)paint.BackgroundColor.SingleVal);
                }
            }

            // background-pattern
            //   Optional string. Interval.
            //   Name of image in sprite to use for drawing image background. For seamless patterns, 
            //   image width and height must be a factor of two (2, 4, 8, …, 512). Note that 
            //   zoom -dependent expressions will be evaluated only at integer zoom levels.
            if (paint.BackgroundPattern != null)
            {
                if (paint.BackgroundPattern.Stops == null && !paint.BackgroundPattern.SingleVal.Contains("{"))
                {
                    var sprite = spriteAtlas.GetSprite(paint.BackgroundPattern.SingleVal);
                    if (sprite != null && sprite.Atlas >= 0)
                        brush.SetFixShader(sprite.ToSKImage().ToShader(SKShaderTileMode.Repeat, SKShaderTileMode.Repeat));
                }
                else
                {
                    brush.SetVariableShader((context) =>
                    {
                        var name = ReplaceFields(paint.BackgroundPattern.Evaluate(context.Zoom), null);

                        var sprite = spriteAtlas.GetSprite(name);
                        if (sprite != null && sprite.Atlas >= 0)
                        {
                            return sprite.ToSKImage().ToShader(SKShaderTileMode.Repeat, SKShaderTileMode.Repeat);
                        }
                        else
                        {
                            // Log information
                            Logging.Logger.Log(Logging.LogLevel.Information, $"Fill pattern {name} not found");
                            // No sprite found
                            return null;
                        }
                    });
                }
            }

            // background-opacity
            //   Optional number. Defaults to 1.
            //   The opacity at which the background will be drawn.
            if (paint?.BackgroundOpacity != null)
            {
                if (paint.BackgroundOpacity.Stops != null)
                {
                    brush.SetVariableOpacity((context) => paint.BackgroundOpacity.Evaluate(context.Zoom));
                }
                else
                {
                    brush.SetFixOpacity(paint.BackgroundOpacity.SingleVal);
                }
            }

            return new List<OMTPaint> { brush };
        }

        public static List<OMTPaint> ConvertRasterLayer(JsonStyleLayer jsonStyleLayer)
        {
            var paint = jsonStyleLayer.Paint;

            var brush = new OMTPaint(jsonStyleLayer.Id);

            brush.SetFixOpacity(1);

            // raster-opacity
            //   Optional number. Defaults to 1.
            //   The opacity at which the image will be drawn.
            if (paint?.RasterOpacity != null)
            {
                if (paint.RasterOpacity.Stops != null)
                {
                    brush.SetVariableOpacity((context) => paint.RasterOpacity.Evaluate(context.Zoom));
                }
                else
                {
                    brush.SetFixOpacity(paint.RasterOpacity.SingleVal);
                }
            }

            // raster-hue-rotate
            //   Optional number. Units in degrees. Defaults to 0.
            //   Rotates hues around the color wheel.

            // raster-brightness-min
            //   Optional number.Defaults to 0.
            //   Increase or reduce the brightness of the image. The value is the minimum brightness.

            // raster-brightness-max
            //   Optional number. Defaults to 1.
            //   Increase or reduce the brightness of the image. The value is the maximum brightness.

            // raster-saturation
            //   Optional number.Defaults to 0.
            //   Increase or reduce the saturation of the image.

            // raster-contrast
            //   Optional number. Defaults to 0.
            //   Increase or reduce the contrast of the image.

            // raster-fade-duration
            //   Optional number.Units in milliseconds.Defaults to 300.
            //   Fade duration when a new tile is added.

            return new List<OMTPaint>() { brush };
        }

        public static List<OMTPaint> ConvertFillLayer(JsonStyleLayer jsonStyleLayer, OMTSpriteAtlas spriteAtlas)
        {
            var layout = jsonStyleLayer?.Layout;
            var paint = jsonStyleLayer?.Paint;

            var hasOutline = false;

            var area = new OMTPaint(jsonStyleLayer.Id);
            var line = new OMTPaint(jsonStyleLayer.Id);

            // Set defaults
            area.SetFixColor(new SKColor(0, 0, 0, 255));
            area.SetFixOpacity(1);
            area.SetFixStyle(SKPaintStyle.Fill);
            line.SetFixColor(new SKColor(0, 0, 0, 255));
            line.SetFixOpacity(1);
            line.SetFixStyle(SKPaintStyle.Stroke);
            line.SetFixStrokeWidth(1);

            // If we don't have a paint, than there isn't anything that we could do
            if (paint == null)
            {
                return new List<OMTPaint>() { area, line };
            }

            // fill-color
            //   Optional color. Defaults to #000000. Disabled by fill-pattern. Exponential.
            //   The color of the filled part of this layer. This color can be specified as 
            //   rgba with an alpha component and the color's opacity will not affect the 
            //   opacity of the 1px stroke, if it is used.
            if (paint.FillColor != null)
            {
                if (paint.FillColor.Stops != null)
                {
                    area.SetVariableColor((context) => jsonStyleLayer.Paint.FillColor.Evaluate(context.Zoom));
                    line.SetVariableColor((context) => jsonStyleLayer.Paint.FillColor.Evaluate(context.Zoom));
                }
                else
                {
                    area.SetFixColor((SKColor)jsonStyleLayer.Paint.FillColor.SingleVal);
                    line.SetFixColor((SKColor)jsonStyleLayer.Paint.FillColor.SingleVal);
                }
            }

            // fill-opacity
            //   Optional number. Defaults to 1. Exponential.
            //   The opacity of the entire fill layer. In contrast to the fill-color, this 
            //   value will also affect the 1px stroke around the fill, if the stroke is used.
            if (paint.FillOpacity != null)
            {
                if (paint.FillOpacity.Stops != null)
                {
                    area.SetVariableOpacity((context) => jsonStyleLayer.Paint.FillOpacity.Evaluate(context.Zoom));
                    line.SetVariableOpacity((context) => jsonStyleLayer.Paint.FillOpacity.Evaluate(context.Zoom));
                }
                else
                {
                    area.SetFixOpacity(jsonStyleLayer.Paint.FillOpacity.SingleVal);
                    line.SetFixOpacity(jsonStyleLayer.Paint.FillOpacity.SingleVal);
                }
            }

            // fill-antialias
            //   Optional boolean. Defaults to true. Interval.
            //   Whether or not the fill should be antialiased.
            if (paint.FillAntialias != null)
            {
                if (paint.FillAntialias.Stops != null)
                {
                    area.SetVariableAntialias((context) => jsonStyleLayer.Paint.FillAntialias.Evaluate(context.Zoom));
                    line.SetVariableAntialias((context) => jsonStyleLayer.Paint.FillAntialias.Evaluate(context.Zoom));
                }
                else
                {
                    area.SetFixAntialias(jsonStyleLayer.Paint.FillAntialias.SingleVal == null ? false : (bool)jsonStyleLayer.Paint.FillAntialias.SingleVal);
                    line.SetFixAntialias(jsonStyleLayer.Paint.FillAntialias.SingleVal == null ? false : (bool)jsonStyleLayer.Paint.FillAntialias.SingleVal);
                }
            }

            // fill-outline-color
            //   Optional color. Disabled by fill-pattern. Requires fill-antialias = true. Exponential. 
            //   The outline color of the fill. Matches the value of fill-color if unspecified.
            if (paint.FillOutlineColor != null)
            {
                hasOutline = true;
                if (paint.FillOutlineColor.Stops != null)
                {
                    line.SetVariableColor((context) => jsonStyleLayer.Paint.FillOutlineColor.Evaluate(context.Zoom));
                }
                else
                {
                    line.SetFixColor((SKColor)jsonStyleLayer.Paint.FillOutlineColor.SingleVal);
                }
            }

            // fill-translate
            //   Optional array. Units in pixels. Defaults to 0,0. Exponential.
            //   The geometry's offset. Values are [x, y] where negatives indicate left and up, 
            //   respectively.

            // TODO: Use matrix of paint object for this

            // fill-translate-anchor
            //   Optional enum. One of map, viewport. Defaults to map. Requires fill-translate. Interval.
            //   Control whether the translation is relative to the map (north) or viewport (screen)

            // TODO: Use matrix of paint object for this

            // fill-pattern
            //   Optional string. Interval.
            //   Name of image in sprite to use for drawing image fills. For seamless patterns, 
            //   image width and height must be a factor of two (2, 4, 8, …, 512).
            if (paint.FillPattern != null)
            {
                // FillPattern needs a color. Instead no pattern is drawn.
                area.SetFixColor(SKColors.Black);

                if (paint.FillPattern.Stops == null && !paint.FillPattern.SingleVal.Contains("{"))
                {
                    area.SetVariableShader((context) =>
                    {
                        var name = paint.FillPattern.SingleVal;

                        var sprite = spriteAtlas.GetSprite(name);
                        if (sprite != null && sprite.Atlas >= 0)
                        {
                            return sprite.ToSKImage().ToShader(SKShaderTileMode.Repeat, SKShaderTileMode.Repeat, SKMatrix.CreateScale(context.Scale, context.Scale));
                        }
                        else
                        {
                            // Log information, that no sprite is found
                            // TODO
                            // Logging.Logger.Log(Logging.LogLevel.Information, $"Fill pattern {name} not found");
                            return null;
                        }
                    });
                }
                else
                {
                    area.SetVariableShader((context) =>
                    {
                        var name = ReplaceFields(jsonStyleLayer.Paint.FillPattern.Evaluate(context.Zoom), context.Tags);

                        var sprite = spriteAtlas.GetSprite(name);
                        if (sprite != null && sprite.Atlas >= 0)
                        {
                            return sprite.ToSKImage().ToShader(SKShaderTileMode.Repeat, SKShaderTileMode.Repeat, SKMatrix.CreateScale(context.Scale, context.Scale));
                        }
                        else
                        {
                            // Log information, that no sprite is found
                            // TODO
                            // Logging.Logger.Log(Logging.LogLevel.Information, $"Fill pattern {name} not found");
                            return null;
                        }
                    });
                }
            }

            // We only have to draw line around areas, when color is different from fill color
            if (hasOutline)
                return new List<OMTPaint>() { area, line };
            else
                return new List<OMTPaint>() { area };
        }

        public static List<OMTPaint> ConvertLineLayer(JsonStyleLayer jsonStyleLayer, OMTSpriteAtlas spriteAtlas)
        {
            var layout = jsonStyleLayer?.Layout;
            var paint = jsonStyleLayer?.Paint;

            var line = new OMTPaint(jsonStyleLayer.Id);

            // Set defaults
            line.SetFixColor(new SKColor(0, 0, 0, 255));
            line.SetFixStyle(SKPaintStyle.Stroke);
            line.SetFixStrokeWidth(1);
            line.SetFixStrokeCap(SKStrokeCap.Butt);
            line.SetFixStrokeJoin(SKStrokeJoin.Miter);

            // If we don't have a paint, than there isn't anything that we could do
            if (paint == null)
            {
                return new List<OMTPaint>() { line };
            }

            // line-cap
            //   Optional enum. One of butt, round, square. Defaults to butt. Interval.
            //   The display of line endings.
            if (layout?.LineCap != null)
            {
                switch (layout.LineCap)
                {
                    case "butt":
                        line.SetFixStrokeCap(SKStrokeCap.Butt);
                        break;
                    case "round":
                        line.SetFixStrokeCap(SKStrokeCap.Round);
                        break;
                    case "square":
                        line.SetFixStrokeCap(SKStrokeCap.Square);
                        break;
                    default:
                        line.SetFixStrokeCap(SKStrokeCap.Butt);
                        break;
                }
            }

            // line-join
            //   Optional enum. One of bevel, round, miter. Defaults to miter.
            //   The display of lines when joining.
            if (layout?.LineJoin != null)
            {
                switch (layout.LineJoin)
                {
                    case "bevel":
                        line.SetFixStrokeJoin(SKStrokeJoin.Bevel);
                        break;
                    case "round":
                        line.SetFixStrokeJoin(SKStrokeJoin.Round);
                        break;
                    case "mitter":
                        line.SetFixStrokeJoin(SKStrokeJoin.Miter);
                        break;
                    default:
                        line.SetFixStrokeJoin(SKStrokeJoin.Miter);
                        break;
                }
            }

            // line-color
            //   Optional color. Defaults to #000000. Disabled by line-pattern. Exponential.
            //   The color with which the line will be drawn.
            if (paint?.LineColor != null)
            {
                if (paint.LineColor.Stops != null)
                {
                    line.SetVariableColor((context) => jsonStyleLayer.Paint.LineColor.Evaluate(context.Zoom));
                }
                else
                {
                    line.SetFixColor((SKColor)jsonStyleLayer.Paint.LineColor.SingleVal);
                }
            }

            // line-width
            //   Optional number.Units in pixels.Defaults to 1. Exponential.
            //   Stroke thickness.
            if (paint?.LineWidth != null)
            {
                if (paint.LineWidth.Stops != null)
                {
                    line.SetVariableStrokeWidth((context) => jsonStyleLayer.Paint.LineWidth.Evaluate(context.Zoom));
                }
                else
                {
                    line.SetFixStrokeWidth(jsonStyleLayer.Paint.LineWidth.SingleVal);
                }
            }

            // line-opacity
            //   Optional number. Defaults to 1. Exponential.
            //   The opacity at which the line will be drawn.
            if (paint?.LineOpacity != null)
            {
                if (paint.LineOpacity.Stops != null)
                {
                    line.SetVariableOpacity((context) => jsonStyleLayer.Paint.LineOpacity.Evaluate(context.Zoom));
                }
                else
                {
                    line.SetFixOpacity(jsonStyleLayer.Paint.LineOpacity.SingleVal);
                }
            }

            // line-dasharray
            //   Optional array. Units in line widths. Disabled by line-pattern. Interval.
            //   Specifies the lengths of the alternating dashes and gaps that form the dash pattern. 
            //   The lengths are later scaled by the line width.To convert a dash length to pixels, 
            //   multiply the length by the current line width.
            if (paint?.LineDashArray != null)
            {
                if (paint.LineDashArray.Stops != null)
                {
                    line.SetVariableDashArray((context) => jsonStyleLayer.Paint.LineDashArray.Evaluate(context.Zoom));
                }
                else
                {
                    line.SetFixDashArray(jsonStyleLayer.Paint.LineDashArray.SingleVal);
                }
            }

            // line-miter-limit
            //   Optional number. Defaults to 2. Requires line-join = miter. Exponential.
            //   Used to automatically convert miter joins to bevel joins for sharp angles.

            // line-round-limit
            //   Optional number. Defaults to 1.05. Requires line-join = round. Exponential.
            //   Used to automatically convert round joins to miter joins for shallow angles.

            // line-translate
            //   Optional array. Units in pixels.Defaults to 0,0. Exponential.
            //   The geometry's offset. Values are [x, y] where negatives indicate left and up, 
            //   respectively.

            // line-translate-anchor
            //   Optional enum. One of map, viewport.Defaults to map. Requires line-translate. Interval.
            //   Control whether the translation is relative to the map (north) or viewport (screen)

            // line-gap-width
            //   Optional number.Units in pixels.Defaults to 0. Exponential.
            //   Draws a line casing outside of a line's actual path.Value indicates the width of 
            //   the inner gap.

            // line-offset
            //   Optional number. Units in pixels. Defaults to 0. Exponential.
            //   The line's offset perpendicular to its direction. Values may be positive or negative, 
            //   where positive indicates "rightwards" (if you were moving in the direction of the line) 
            //   and negative indicates "leftwards".

            // line-blur
            //   Optional number. Units in pixels.Defaults to 0. Exponential.
            //   Blur applied to the line, in pixels.

            // line-pattern
            //   Optional string. Interval.
            //   Name of image in sprite to use for drawing image lines. For seamless patterns, image 
            //   width must be a factor of two (2, 4, 8, …, 512).

            return new List<OMTPaint>() { line };
        }

        public static OMTSymbolFactory ConvertSymbolLayer(JsonStyleLayer jsonStyleLayer, OMTSpriteAtlas spriteAtlas)
        {
            var layout = jsonStyleLayer?.Layout;
            var paint = jsonStyleLayer?.Paint;

            // If we don't have a paint, than there isn't anything that we could do
            //if (paint == null)
            //{
            //    return OMTSymbolFactory.Default;
            //}

            OMTSymbolFactory symbolStyler = new OMTSymbolFactory(jsonStyleLayer.Id, spriteAtlas);

            if (layout?.Visibility != null)
            {
                // TODO
                symbolStyler.IsVisible = layout.Visibility == "visible";
            }

            // icon-allow-overlap
            //   Optional boolean. Defaults to false. Requires icon-image. Interval.
            //   If true, the icon will be visible even if it collides with other previously drawn symbols.
            if (layout?.IconImage != null && layout?.IconAllowOverlap != null)
            {
                // TODO
                symbolStyler.IconAllowOverlap = layout.IconAllowOverlap;
            }

            // icon-anchor
            //   Optional enum. One of center, left, right, top, bottom, top-left, top-right, bottom-left, 
            //   bottom-right. Defaults to center. Requires text-field. Interval.
            //   Part of the text placed closest to the anchor.
            if (layout?.IconImage != null && layout?.IconAnchor != null)
            {
                symbolStyler.IconAnchor = layout.IconAnchor.ToDirection();
            }

            // icon-color
            //   Optional color. Defaults to #000000. Requires icon-image. Exponential.
            //   The color of the icon. This can only be used with sdf icons.
            if (layout?.IconImage != null && layout?.IconColor != null)
            {
                symbolStyler.IconColor = layout.IconColor;
            }

            // icon-halo-blur
            //   Optional number. Units in pixels. Defaults to 0. Requires icon-image. Exponential.
            //   Fade out the halo towards the outside.
            if (layout?.IconImage != null && layout?.IconHaloBlur != null)
            {
                symbolStyler.IconHaloBlur = layout.IconHaloBlur;
            }

            // icon-halo-color
            //   Optional color. Defaults to rgba(0, 0, 0, 0). Requires icon-image. Exponential.
            //   The color of the icon's halo. Icon halos can only be used with sdf icons.
            if (layout?.IconImage != null && layout?.IconHaloColor != null)
            {
                symbolStyler.IconHaloColor = layout.IconHaloColor;
            }

            // icon-halo-width
            //   Optional number. Units in pixels. Defaults to 0. Requires icon-image. Exponential.
            //   Distance of halo to the icon outline.
            if (layout?.IconImage != null && layout?.IconHaloWidth != null)
            {
                symbolStyler.IconHaloWidth = layout.IconHaloWidth;
            }

            // icon-ignore-placement
            //   Optional boolean. Defaults to false. Requires icon-image. Interval.
            //   If true, other symbols can be visible even if they collide with the icon.
            if (layout?.IconImage != null && layout?.IconIgnorePlacement != null)
            {
                symbolStyler.IconIgnorePlacement = layout.IconIgnorePlacement;
            }

            // icon-image
            //   Optional string.
            //   A string with { tokens } replaced, referencing the data property to pull from. Interval.
            if (layout?.IconImage != null)
            {
                // TODO: Get the right list (see https://docs.mapbox.com/mapbox-gl-js/style-spec/types/#resolvedimage)
                symbolStyler.IconImage = layout.IconImage;
            }

            // icon-keep-upright
            //   Optional boolean. Defaults to false. Requires icon-image. Requires icon-rotation-alignment = "map". Interval.
            //   Requires symbol-placement = "line" or "line-center".
            //   If true, the icon may be flipped to prevent it from being rendered upside-down.
            if (layout?.IconImage != null && 
                layout?.IconRotationAlignment?.ToLower() == "map" &&
                //(layout?.SymbolPlacement?.ToLower() == "line" || layout?.SymbolPlacement?.ToLower() == "line-center") &&
                layout?.IconKeepUpright != null)
            {
                symbolStyler.IconKeepUpright = layout.IconKeepUpright;
            }

            // icon-offset
            //   Optional array. Defaults to 0,0. Requires icon-image. Exponential.
            //   Offset distance of icon from its anchor. Positive values indicate right and down, 
            //   while negative values indicate left and up.
            if (layout?.IconImage != null && layout?.IconOffset != null)
            {
                // TODO: Is a stopped value
                symbolStyler.IconOffset = new Vector(layout.IconOffset[0], layout.IconOffset[1]);
            }

            // icon-opacity
            //   Optional number. Defaults to 1. Requires icon-image. Exponential.
            //   The opacity at which the icon will be drawn.
            if (layout?.IconImage != null && layout?.IconOpacity != null)
            {
                symbolStyler.IconOpacity = layout.IconOpacity;
            }

            // icon-optional
            //   Optional boolean. Defaults to false. Requires icon-image. Requires text-field. Interval.
            //   If true, text will display without their corresponding icons when the icon collides 
            //   with other symbols and the text does not.
            if (layout?.IconImage != null && layout?.TextField != null && layout?.IconOptional != null)
            {
                symbolStyler.IconOptional = layout.IconOptional;
            }

            // icon-padding
            //   Optional number. Units in pixels. Defaults to 2. Requires icon-image. Exponential.
            //   Size of the additional area around the icon bounding box used for detecting symbol collisions.
            if (layout?.IconImage != null && layout?.IconPadding != null)
            {
                symbolStyler.IconPadding = layout.IconPadding;
            }

            // icon-pitch-alignment
            //   Optional enum. One of "map", "viewport", "auto". Defaults to "auto". Requires icon-image. 
            if (layout?.IconImage != null && layout?.IconPitchAlignment != null)
            {
                symbolStyler.IconPitchAlignment = layout.IconPitchAlignment.ToMapAlignment();
            }

            // icon-rotate
            //   Optional number. Units in degrees. Defaults to 0. Requires icon-image. Exponential.
            //   Rotates the icon clockwise.
            if (layout?.IconImage != null && layout?.IconRotate != null)
            {
                symbolStyler.IconRotate = layout.IconRotate;
            }

            // icon-rotation-alignment
            //   Optional enum. One of map, viewport. Defaults to viewport. Requires icon-image. Interval.
            //   Orientation of icon when map is rotated.
            if (layout?.IconImage != null && layout?.IconRotationAlignment != null)
            {
                symbolStyler.IconRotationAlignment = layout.IconRotationAlignment.ToMapAlignment();
            }

            // icon-size
            //   Optional number. Defaults to 1. Requires icon-image. Exponential.
            //   Scale factor for icon. 1 is original size, 3 triples the size.
            if (layout?.IconImage != null && layout?.IconSize != null)
            {
                symbolStyler.IconSize = layout.IconSize;
            }

            // icon-text-fit
            //   Optional enum. One of "none", "width", "height", "both". Defaults to "none". 
            //   Requires icon-image. Requires text-field. 
            if (layout?.IconImage != null && layout?.TextField != null && layout?.IconTextFit != null)
            {
                symbolStyler.IconTextFit = layout.IconTextFit.ToTextFit();
            }

            // icon-text-fit-padding
            //   Optional array of numbers. Units in pixels.Defaults to[0, 0, 0, 0]. Requires icon-image.
            //   Requires text - field.Requires icon-text-fit to be "both", or "width", or "height".
            if (layout?.IconImage != null && 
                layout?.TextField != null &&
                (layout?.IconTextFit?.ToLower() == "both" || layout?.IconTextFit?.ToLower() == "width" || layout?.IconTextFit?.ToLower() == "height") &&
                layout?.IconTextFitPadding != null)
            {
                symbolStyler.IconTextFitPadding = new MRect(layout.IconTextFitPadding[0], layout.IconTextFitPadding[1], layout.IconTextFitPadding[2], layout.IconTextFitPadding[3]);
            }

            // icon-translate
            //   Optional array. Units in pixels. Defaults to 0, 0. Requires icon-image. Exponential.
            //   Distance that the icon's anchor is moved from its original placement.
            //   Positive values indicate right and down, while negative values indicate left and up.
            if (layout?.IconImage != null && layout?.IconTranslate != null)
            {
                // TODO: Is a stopped value
                symbolStyler.IconTranslate = new Vector(layout.IconTranslate[0], layout.IconTranslate[1]);
            }

            // icon-translate-anchor
            //   Optional enum. One of "map", "viewport". Defaults to "map". Requires icon-image. 
            //   Requires icon-translate. Control whether the translation is relative to the 
            //   map(north) or viewport(screen).
            if (layout?.IconImage != null &&
                layout?.IconTranslate != null &&
                layout?.IconTranslateAnchor != null)
            {
                symbolStyler.IconTranslateAnchor = layout.IconTranslateAnchor.ToMapAlignment();
            }

            // symbol-avoid-edges
            //   Optional boolean. Defaults to false. Interval.
            //   If true, the symbols will not cross tile edges to avoid mutual collisions.
            //   Recommended in layers that don't have enough padding in the vector tile to prevent 
            //   collisions, or if it is a point symbol layer placed after a line symbol layer.
            if (layout?.SymbolAvoidEdges != null)
            {
                symbolStyler.SymbolAvoidEdges = layout.SymbolAvoidEdges;
            }

            // symbol-placement
            //   Optional enum. One of "point", "line" or "line-center". Defaults to "point". Interval.
            //   Label placement relative to its geometry. "line" can only be used on 
            //   LineStrings and Polygons.
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#layout-symbol-symbol-placement
            if (layout?.SymbolPlacement != null)
            {
                symbolStyler.SymbolPlacement = layout.SymbolPlacement; //.ToPlacement();
            }

            // symbol-sort-key
            //   Optional number. Sorts features in ascending order based on this value.
            //   Features with a higher sort key will appear above features with a lower 
            //   sort key when they overlap. Features with a lower sort key will have 
            //   priority over other features when doing placement.
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#layout-symbol-symbol-sort-key
            if (layout?.SymbolSortKey != null)
            {
                symbolStyler.SymbolSortKey = layout.SymbolSortKey;
            }

            // symbol-spacing
            //   Optional number greater than or equal to 1. Units in pixels. 
            //   Defaults to 250. Requires symbol-placement to be "line"
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#layout-symbol-symbol-spacing
            if (layout?.SymbolPlacement != null && layout?.SymbolSpacing != null)
            {
                symbolStyler.SymbolSpacing = layout.SymbolSpacing;
            }

            // symbol-z-order
            //   Optional enum. One of "auto", "viewport-y", "source". Defaults to "auto".
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#layout-symbol-symbol-z-order
            if (layout?.SymbolZOrder != null)
            {
                symbolStyler.SymbolZOrder = layout.SymbolZOrder.ToZOrder();
            }

            // text-allow-overlap
            //   Optional boolean. Defaults to false. Requires text-field. Interval.
            //   If true, the text will be visible even if it collides with other previously drawn symbols.
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#layout-symbol-text-allow-overlap
            if (layout?.TextAllowOverlap != null)
            {
                symbolStyler.TextAllowOverlap = layout.TextAllowOverlap;
            }

            // text-anchor
            //   Optional enum. One of "center", "left", "right", "top", "bottom", "top-left", "top-right", 
            //   "bottom-left", "bottom-right". Defaults to "center". Requires text-field. Disabled by 
            //   text-variable-anchor. 
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#layout-symbol-text-anchor
            if (layout?.TextField != null && layout?.TextVariableAnchor == null && layout?.TextAnchor != null)
            {
                symbolStyler.TextAnchor = layout.TextAnchor.ToDirection();
            }

            // text-color
            //   Optional color. Defaults to #000000. Requires text-field. Exponential.
            //   The color with which the text will be drawn.
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#paint-symbol-text-color
            if (layout?.TextField != null && paint?.TextColor != null)
            {
                symbolStyler.TextColor = paint.TextColor;
            }

            // text-field
            //   Optional string. Interval.
            //   Value to use for a text label. Feature properties are specified using tokens like {field_name}.
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#layout-symbol-text-field
            if (layout?.TextField != null)
            {
                symbolStyler.TextField = layout.TextField;
            }

            // text-font
            //   Optional array. Defaults to "Open Sans Regular", "Arial Unicode MS Regular". Requires text-field.
            //   Font stack to use for displaying text.
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#layout-symbol-text-font
            if (layout?.TextField != null && layout?.TextFont != null)
            {
                var fontName = string.Empty;

                foreach (var font in layout.TextFont)
                {
                    // TODO: Check, if font exists
                    symbolStyler.TextFont.Add(font.ToString());
                }
            }

            // text-halo-blur
            //   Optional number. Units in pixels. Defaults to 0. Requires text-field. Exponential.
            //   Fade out the halo towards the outside.
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#paint-symbol-text-halo-blur
            if (layout?.TextField != null && paint?.TextHaloBlur != null)
            {
                symbolStyler.TextHaloBlur = paint.TextHaloBlur;
            }

            // text-halo-color
            //   Optional color. Defaults to rgba(0, 0, 0, 0). Requires text-field. Exponential.
            //   The color of the text's halo.
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#paint-symbol-text-halo-color
            if (layout?.TextField != null && paint?.TextHaloColor != null)
            {
                symbolStyler.TextHaloColor = paint.TextHaloColor;
            }

            // text-halo-width
            //   Optional number. Units in pixels. Defaults to 0. Requires text-field. Exponential.
            //   Distance of halo to the text outline.
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#paint-symbol-text-halo-width
            if (layout?.TextField != null && paint?.TextHaloWidth != null)
            {
                symbolStyler.TextHaloWidth = paint.TextHaloWidth;
            }

            // text-ignore-placement
            //   Optional boolean. Defaults to false. Requires text-field. Interval.
            //   If true, other symbols can be visible even if they collide with the text.
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#layout-symbol-text-ignore-placement
            if (layout?.TextField != null && layout?.TextIgnorePlacement != null)
            {
                symbolStyler.TextIgnorePlacement = layout.TextIgnorePlacement;
            }

            // text-justify
            //   Optional enum. One of "auto", "left", "center", "right". Defaults to "center". 
            //   Requires text-field. Interval. Text justification options.
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#layout-symbol-text-justify
            if (layout?.TextField != null && layout?.TextJustify != null)
            {
                symbolStyler.TextJustify = layout.TextJustify.ToTextJustify();
            }

            // text-keep-upright
            //   Optional boolean. Defaults to true. Requires text-field. Requires text-rotation-alignment = map.
            //   Requires symbol-placement = line. Interval.
            //   If true, the text may be flipped vertically to prevent it from being rendered upside-down.
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#layout-symbol-text-keep-upright
            if (layout?.TextField != null &&
                layout?.TextRotationAlignment?.ToLower() == "map" &&
                //(layout?.SymbolPlacement?.ToLower() == "line" || layout?.SymbolPlacement?.ToLower() == "line-center") &&
                layout?.TextKeepUpright != null)
            {
                symbolStyler.TextKeepUpright = layout.TextKeepUpright;
            }

            // text-letter-spacing
            //   Optional number. Units in em. Defaults to 0. Requires text-field. Exponential.
            //   Text tracking amount.
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#layout-symbol-text-letter-spacing
            if (layout?.TextField != null && layout?.TextLetterSpacing != null)
            {
                symbolStyler.TextLetterSpacing = layout.TextLetterSpacing;
            }

            // text-line-height
            //   Optional number. Units in em. Defaults to 1.2. Requires text-field. Exponential.
            //   Text leading value for multi-line text.
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#layout-symbol-text-line-height
            if (layout?.TextField != null && layout?.TextLineHeight != null)
            {
                symbolStyler.TextLineHeight = layout.TextLineHeight;
            }

            // text-max-angle
            //   Optional number. Units in degrees. Defaults to 45. Requires text-field. 
            //   Requires symbol-placement = line. Exponential.
            //   Maximum angle change between adjacent characters.
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#layout-symbol-text-max-angle
            if (layout?.TextField != null &&
                //(layout?.SymbolPlacement?.ToLower() == "line" || layout?.SymbolPlacement?.ToLower() == "line-center") &&
                layout?.TextMaxAngle != null)
            {
                symbolStyler.TextMaxAngle = layout.TextMaxAngle;
            }

            // text-max-width
            //   Optional number. Units in em. Defaults to 10. Requires text-field. Exponential.
            //   The maximum line width for text wrapping.
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#layout-symbol-text-max-width
            if (layout?.TextField != null && layout?.TextMaxWidth != null)
            {
                symbolStyler.TextMaxWidth = layout.TextMaxWidth;
            }

            // text-offset
            //   Optional array. Units in em. Defaults to 0,0. Requires text-field. Exponential.
            //   Offset distance of text from its anchor. Positive values indicate right and down, 
            //   while negative values indicate left and up.
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#layout-symbol-text-offset
            if (layout?.TextField != null && layout?.TextOffset != null)
            {
                symbolStyler.TextOffset = new Vector(layout.TextOffset[0], layout.TextOffset[1]);
            }

            // text-opacity
            //   Optional number. Defaults to 1. Requires text-field. Exponential.
            //   The opacity at which the text will be drawn.
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#paint-symbol-text-opacity
            if (layout?.TextField != null && paint?.TextOpacity != null)
            {
                symbolStyler.TextOpacity = paint.TextOpacity;
            }

            // text-optional
            //   Optional boolean. Defaults to false. Requires text-field. Requires icon-image. Interval.
            //   If true, icons will display without their corresponding text when the text collides with 
            //   other symbols and the icon does not.
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#layout-symbol-text-optional
            if (layout?.TextField != null && layout?.IconImage != null && layout?.TextOptional != null)
            {
                symbolStyler.TextOptional = layout.TextOptional;
            }

            // text-padding
            //   Optional number. Units in pixels. Defaults to 2. Requires text-field. Exponential.
            //   Size of the additional area around the text bounding box used for detecting symbol collisions.
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#layout-symbol-text-padding
            if (layout?.TextField != null && layout?.TextPadding != null)
            {
                symbolStyler.TextPadding = layout.TextPadding;
            }

            // text-pitch-alignment
            //   Optional enum. One of "map", "viewport", "auto". Defaults to "auto". Requires text-field. 
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#layout-symbol-text-pitch-alignment
            if (layout?.TextField != null && layout?.TextPitchAlignment != null)
            {
                symbolStyler.TextPitchAlignment = layout.TextPitchAlignment.ToMapAlignment();
            }

            // text-radial-offset
            //   Optional number. Units in em. Defaults to 0. Requires text-field. Exponential.
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#layout-symbol-text-radial-offset
            if (layout?.TextField != null && layout?.TextRadialOffset != null)
            {
                symbolStyler.TextRadialOffset = layout.TextRadialOffset;
            }

            // text-rotate
            //   Optional number. Units in degrees. Defaults to 0. Requires text-image. Exponential.
            //   Rotates the text clockwise.
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#layout-symbol-text-rotate
            if (layout?.TextField != null && layout?.TextRotate != null)
            {
                symbolStyler.TextRotate = layout.TextRotate;
            }

            // text-rotation-alignment
            //   Optional enum. One of map, viewport and auto. Defaults to viewport. Requires text-field. Interval.
            //   Orientation of icon when map is rotated.
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#layout-symbol-text-rotation-alignment
            if (layout?.TextField != null && layout?.TextRotationAlignment != null)
            {
                symbolStyler.TextRotationAlignment = layout.TextRotationAlignment.ToMapAlignment();
            }

            // text-size
            //   Optional number greater than or equal to 0. Units in pixels. Defaults to 16. Requires text-field. 
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#layout-symbol-text-size
            if (layout?.TextField != null && layout?.TextSize != null)
            {
                symbolStyler.TextSize = layout.TextSize;
            }

            // text-transform
            //   Optional enum. One of none, uppercase, lowercase. Defaults to none. Requires text-field. Interval.
            //   Specifies how to capitalize text, similar to the CSS text-transform property.
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#layout-symbol-text-transform
            if (layout?.TextField != null && layout?.TextTransform != null)
            {
                symbolStyler.TextTransform = layout.TextTransform.ToTextTransform();
            }

            // text-translate
            //   Optional array. Units in pixels. Defaults to 0, 0. Requires text-field. Exponential.
            //   Distance that the icon's anchor is moved from its original placement.
            //   Positive values indicate right and down, while negative values indicate left and up.
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#paint-symbol-text-translate
            if (layout?.TextField != null && layout?.TextTranslate != null)
            {
                // TODO: Is a stopped value
                symbolStyler.TextTranslate = new Vector(layout.TextTranslate[0], layout.TextTranslate[1]);
            }

            // text-translate-anchor
            //   Optional enum. One of "map", "viewport". Defaults to "map". Requires icon-image. 
            //   Requires text-translate. Control whether the translation is relative to the 
            //   map (north) or viewport (screen).
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#paint-symbol-text-translate-anchor
            if (layout?.TextField != null &&
                layout?.TextTranslate != null &&
                layout?.TextTranslateAnchor != null)
            {
                symbolStyler.TextTranslateAnchor = layout.TextTranslateAnchor.ToMapAlignment();
            }

            // text-variable-anchor
            //   Optional array of enums. One of "center", "left", "right", "top", "bottom", 
            //   "top-left", "top-right", "bottom-left", "bottom-right". Requires text-field. 
            //   Requires symbol-placement to be "point". 
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#layout-symbol-text-variable-anchor
            if (layout?.TextField != null &&
                layout?.SymbolPlacement != null &&
                //layout.SymbolPlacement.ToLower() == "point" &&
                layout?.TextVariableAnchor != null)
            {
                foreach(var alignment in layout?.TextVariableAnchor)
                    symbolStyler.TextVariableAnchor.Add(alignment.ToMapAlignment());
            }

            // text-writing-mode
            //   Optional array of enums. One of "horizontal", "vertical". Requires text-field. 
            //   Requires symbol-placement to be "point".  
            //   See https://docs.mapbox.com/mapbox-gl-js/style-spec/layers/#layout-symbol-text-writing-mode
            if (layout?.TextField != null &&
                layout?.SymbolPlacement != null &&
                //layout.SymbolPlacement.ToLower() == "point" &&
                layout?.TextWritingMode != null)
            {
                foreach (var orientation in layout?.TextWritingMode)
                    symbolStyler.TextWritingMode.Add(orientation.ToOrientation());
            }

            // Now create default layout settings for this symbolStyler
            symbolStyler.Update();

            return symbolStyler;
        }

        static Regex regExFields = new Regex(@"\{(.*?)\}", (RegexOptions)8);

        /// <summary>
        /// Replace all fields in string with values
        /// </summary>
        /// <param name="text">String with fields to replace</param>
        /// <param name="tags">Tags to replace fields with</param>
        /// <returns></returns>
        public static string ReplaceFields(string text, TagsCollection tags)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var result = text;

            var match = regExFields.Match(text);

            while (match.Success)
            {
                var field = match.Groups[1].Captures[0].Value;

                // Search field
                var replacement = string.Empty;

                if (tags.ContainsKey(field))
                    replacement = tags[field].ToString();

                // Replace field with new value
                result = result.Replace(match.Groups[0].Captures[0].Value, replacement);

                // Check for next field
                match = match.NextMatch();
            };

            return result;
        }
    }
}
