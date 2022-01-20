// RichTextKit
// Copyright © 2019-2020 Topten Software. All Rights Reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may 
// not use this product except in compliance with the License. You may obtain 
// a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
// License for the specific language governing permissions and limitations 
// under the License.

using SkiaSharp;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayers.OpenMapTiles.Utilities
{
    /// <summary>
    /// The FontMapper class is responsible for mapping style typeface information
    /// to an SKTypeface.
    /// </summary>
    public class FontMapper : Topten.RichTextKit.FontMapper
    {
        private List<SKTypeface> _additionalTypfaces = new List<SKTypeface>();

        /// <summary>
        /// Constructs a new FontMapper instance
        /// </summary>
        public FontMapper()
        {
        }

        /// <summary>
        /// Maps a given style to a specific typeface
        /// </summary>
        /// <param name="style">The style to be mapped</param>
        /// <param name="ignoreFontVariants">Indicates the mapping should ignore font variants (use to get font for ellipsis)</param>
        /// <returns>A mapped typeface</returns>
        public override SKTypeface TypefaceFromStyle(Topten.RichTextKit.IStyle style, bool ignoreFontVariants)
        {
            // Extra weight for superscript/subscript
            int extraWeight = 0;
            if (!ignoreFontVariants && (style.FontVariant == Topten.RichTextKit.FontVariant.SuperScript || style.FontVariant == Topten.RichTextKit.FontVariant.SubScript))
            {
                extraWeight += 100;
            }

            foreach (var typeface in _additionalTypfaces)
            {
                if (typeface.FamilyName == style.FontFamily && 
                    typeface.FontWeight == style.FontWeight + extraWeight &&
                    (SKFontStyleWidth)typeface.FontWidth == style.FontWidth &&
                    typeface.FontSlant == (style.FontItalic ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright))
                {
                    return typeface;
                }
            }

            // Get the typeface
            return SKTypeface.FromFamilyName(
                style.FontFamily,
                (SKFontStyleWeight)(style.FontWeight + extraWeight),
                style.FontWidth,
                style.FontItalic ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright
                ) ?? SKTypeface.CreateDefault();
        }

        /// <summary>
        /// The default font mapper instance.  
        /// </summary>
        /// <remarks>
        /// The default font mapper is used by any TextBlocks that don't 
        /// have an explicit font mapper set (see the <see cref="TextBlock.FontMapper"/> property).
        /// 
        /// Replacing the default font mapper allows changing the font mapping
        /// for all text blocks that don't have an explicit mapper assigned.
        /// </remarks>
        static FontMapper()
        {
            // Install self as the default RichTextKit font mapper
            if (!(Topten.RichTextKit.FontMapper.Default is FontMapper))
                Topten.RichTextKit.FontMapper.Default = new FontMapper();
        }

        public void Add(SKTypeface typeface)
        {
            if (!_additionalTypfaces.Contains(typeface))
            {
                _additionalTypfaces.Add(typeface);
            }
        }
    }
}
