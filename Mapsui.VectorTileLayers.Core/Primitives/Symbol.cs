using BruTile;
using Mapsui.VectorTileLayers.Core.Enums;
using Mapsui.VectorTileLayers.Core.Interfaces;
using RBush;
using SkiaSharp;

namespace Mapsui.VectorTileLayers.Core.Primitives
{
    public abstract class Symbol : SKDrawable, ISpatialData
    {
        protected Envelope _envelope;

        /// <summary>
        /// Id of this symbol
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Index of tile to which this symbol belongs
        /// </summary>
        public virtual TileIndex Index { 
            get; 
            set; }

        /// <summary>
        /// Class of this symbol
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// Subclass of this symbol
        /// </summary>
        public string Subclass { get; set; }

        /// <summary>
        /// Name of this symbol with correct culture
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Rank of this symbol lower numbers are importanter
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Flag to show, if this symbol is visible or not
        /// </summary>
        public bool IsVisible { get; set; } = false;

        /// <summary>
        /// Point in tile coordinates
        /// </summary>
        public MPoint Point { get; set; }

        /// <summary>
        /// Anchor in pixel
        /// </summary>
        public MPoint Anchor { get; set; }

        /// <summary>
        /// Offset in pixel
        /// </summary>
        public MPoint Offset { get; set; }

        /// <summary>
        /// Padding around symbol
        /// </summary>
        public float Padding { get; set; }

        /// <summary>
        /// Ignore colliding other symbols
        /// </summary>
        public bool IgnorePlacement { get; set; }

        /// <summary>
        /// Alignment relative to map or viewport
        /// </summary>
        public MapAlignment Alignment { get; set; }

        /// <summary>
        /// Rotation of symbol
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// VectorPaint used to draw this symbol
        /// </summary>
        public IVectorPaint Paint { get; set; }

        public ref readonly Envelope Envelope => ref _envelope;

        public abstract void Update(EvaluationContext context);

        public abstract Symbol TreeSearch(RBush<Symbol> tree);

        public abstract void CalcEnvelope(float scale, float rotation, MPoint offset);

        public abstract void AddEnvelope(RBush<Symbol> tree);

        public abstract void Draw(SKCanvas canvas, EvaluationContext context);
    }
}
