using Mapsui.Styles;

namespace Mapsui.VectorTileLayers.Core.Interfaces
{
    /// <summary>
    /// A SpriteAtlas holds one or more ISprite
    /// </summary>
    public interface ISpriteAtlas
    {
        /// <summary>
        /// Return a sprite for a given name
        /// </summary>
        /// <param name="name">Name of sprite</param>
        /// <returns>Sprite or null, if the atlas doesn't contain a sprite with this name</returns>
        Sprite GetSprite(string name);

        /// <summary>
        /// Add a sprite to the atlas
        /// </summary>
        /// <remarks>If the atlas already contains a sprite with the same name, than replace it.</remarks>
        /// <param name="name">Name of sprite</param>
        /// <param name="sprite">Sprite</param>
        void AddSprite(string name, Sprite sprite);
    }
}
