using BruTile;
using Mapsui.VectorTileLayers.Core.Enums;
using Mapsui.VectorTileLayers.Core.Interfaces;
using RBush;
using SkiaSharp;
using System.Collections.Generic;

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
        /// Possible anchor that could be used with this symbol
        /// </summary>
        public List<MPoint> PossibleAnchors { get; set; }

        /// <summary>
        /// Anchor to use
        /// </summary>
        public MPoint Anchor { get; set; }

        /// <summary>
        /// Type of anchor
        /// </summary>
        public AnchorType AnchorType { get; set; } = AnchorType.Fixed;

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
        /// Style layer of this symbol
        /// </summary>
        public virtual IStyleLayer Style { get; set; }

        /// <summary>
        /// VectorPaint used to draw this symbol
        /// </summary>
        public IVectorPaint Paint { get; set; }

        public ref readonly Envelope Envelope => ref _envelope;

        public abstract void Update(EvaluationContext context);

        public abstract Symbol TreeSearch(RBush<Symbol> tree);

        public abstract void CalcEnvelope(float scale, float rotation, MPoint offset);

        public abstract void AddEnvelope(RBush<Symbol> tree);

        /// <summary>
        /// Try to place this symbol regarding the other symbols already in the tree
        /// </summary>
        /// <param name="tree">Tree with other already placed symbols</param>
        /// <param name="context">Context to use while try to place the symbol</param>
        public abstract void Render(RBush<Symbol> tree, EvaluationContext context);

        /// <summary>
        /// Draw this symbol on canvas
        /// </summary>
        /// <param name="canvas">Canvas to draw the symbol on</param>
        /// <param name="context">Context to use while drawing the symbol</param>
        public abstract void Draw(SKCanvas canvas, EvaluationContext context);
    }
}
