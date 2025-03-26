using BruTile;
using Mapsui.Layers;
using SkiaSharp;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayers.OpenMapTiles
{
    /// <summary>
    /// Class holding all relevant data from the Mapbox GL Json Style File
    /// </summary>
    public class OMTStyleFile
    {
        public OMTStyleFile(string name, int version)
        {
            Name = name;
            Version = version;
        }

        /// <summary>
        /// Name of this style file
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Version of this style file
        /// </summary>
        public int Version { get; }

        /// <summary>
        /// Center of map as provided in the style file
        /// </summary>
        public MPoint Center { get; internal set; }

        /// <summary>
        /// Sources is a list of all ITileSources, that this style file provides
        /// </summary>
        public List<ITileSource> TileSources { get; } = new List<ITileSource>();

        /// <summary>
        /// List of all ILayers, that this style file contains
        /// </summary>
        public List<ILayer> TileLayers { get; } = new List<ILayer>();

        /// <summary>
        /// SpriteAtlas containing all sprites of this style file
        /// </summary>
        public OMTSpriteAtlas SpriteAtlas { get; } = new OMTSpriteAtlas();

        /// <summary>
        /// GlyphAtlas containing all glyphs for this style file
        /// </summary>
        public object GlyphAtlas { get; internal set; }

        public Dictionary<string, SKTypeface> SpecialFonts { get; internal set; }
    }
}
