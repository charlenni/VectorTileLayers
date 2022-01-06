using Mapsui.Styles;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Mapsui.VectorTileLayer.Core.Interfaces;

namespace Mapsui.VectorTileLayer.MapboxGL
{
    public class MGLSpriteAtlas : ISpriteAtlas
    {
        Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

        /// <summary>
        /// Add a MapboxGL source to atlas
        /// </summary>
        /// <param name="source">Url or path name to get sprites definition and atlas from</param>
        public void AddSpriteSource(string source, Assembly assemblyToUse)
        {
            Stream streamJson;
            Stream streamAtlas;

            // First check for @2x
            var nameJson = source + "@2x.json";
            var nameAtlas = source + "@2x.png";

            (streamJson, streamAtlas) = GetStreams(nameJson, nameAtlas, assemblyToUse);

            if (streamJson == null || streamAtlas == null)
            {
                // Perhaps there are no @2x versions
                nameJson = source + ".json";
                nameAtlas = source + ".png";

                (streamJson, streamAtlas) = GetStreams(nameJson, nameAtlas, assemblyToUse);
            }

            if (streamJson != null && streamAtlas != null)
            {
                CreateSprites(streamJson, streamAtlas);
            }
        }

        /// <summary>
        /// Return a sprite for a given name
        /// </summary>
        /// <param name="name">Name of sprite</param>
        /// <returns>Sprite or null, if the atlas doesn't contain a sprite with this name</returns>
        public Sprite GetSprite(string name)
        {
            if (sprites.ContainsKey(name))
                return sprites[name];

            return null;
        }

        /// <summary>
        /// Add a sprite to the atlas
        /// </summary>
        /// <remarks>If the atlas already contains a sprite with the same name, than replace it.</remarks>
        /// <param name="name">Name of sprite</param>
        /// <param name="sprite">Sprite</param>
        public void AddSprite(string name, Sprite sprite)
        {
            sprites[name] = sprite;
        }

        /// <summary>
        /// Create sprite atlas from a stream
        /// </summary>
        /// <param name="jsonStream">Stream with Json sprite file information</param>
        /// <param name="bitmapAtlasStream">Stream with containing bitmap with sprite atlas bitmap</param>
        public void CreateSprites(Stream jsonStream, Stream bitmapAtlasStream)
        {
            string json;

            using (var reader = new StreamReader(jsonStream))
            {
                json = reader.ReadToEnd();
            }

            var bitmapAtlasData = SKData.Create(bitmapAtlasStream);
            var bitmapAtlas = SKImage.FromEncodedData(bitmapAtlasData);

            CreateSprites(json, bitmapAtlas);
        }

        /// <summary>
        /// Create sprite atlas from a string
        /// </summary>
        /// <param name="json">String with Json sprite file information</param>
        /// <param name="atlas">SKImage of sprite atlas</param>
        private void CreateSprites(string json, SKImage atlas)
        {
            var spriteAtlasId = BitmapRegistry.Instance.Register(atlas);
            var sprites = JsonConvert.DeserializeObject<Dictionary<string, Json.JsonSprite>>(json);

            foreach (var sprite in sprites)
            {
                // Extract sprite from atlas
                AddSprite(sprite.Key, new MGLSprite(sprite, spriteAtlasId));
            }
        }

        private (Stream, Stream) GetStreams(string nameJson, string nameAtlas, Assembly assemblyToUse)
        {
            Stream streamJson;
            Stream streamAtlas;

            if (nameJson.StartsWith("http"))
            {
                (streamJson, streamAtlas) = GetStreamsFromUrl(nameJson, nameAtlas);
            }
            else if (nameJson.StartsWith("file://"))
            {
                nameJson = nameJson.Substring(7);
                nameAtlas = nameAtlas.Substring(7);

                (streamJson, streamAtlas) = GetStreamsFromFile(nameJson, nameAtlas);
            }
            else if (nameJson.StartsWith("embedded://"))
            {
                nameJson = nameJson.Substring(11).Replace('/', '.');
                nameAtlas = nameAtlas.Substring(11).Replace('/', '.');

                (streamJson, streamAtlas) = GetStreamsFromResource(nameJson, nameAtlas, assemblyToUse);
            }
            else
            {
                // Unknown source type, so do nothing
                throw new NotImplementedException("Unknown URL for sprite");
            }

            return (streamJson, streamAtlas);
        }

        private (Stream streamJson, Stream streamAtlas) GetStreamsFromResource(string nameJson, string nameAtlas, Assembly assemblyToUse)
        {
            Stream streamJson = null;
            Stream streamAtlas = null;

            var resourceNames = assemblyToUse?.GetManifestResourceNames();

            var resourceName = resourceNames?.FirstOrDefault(s => s.EndsWith(nameJson, StringComparison.CurrentCultureIgnoreCase));

            if (!string.IsNullOrEmpty(resourceName))
            {
                streamJson = assemblyToUse.GetManifestResourceStream(resourceName);
            }

            resourceName = resourceNames?.FirstOrDefault(s => s.EndsWith(nameAtlas, StringComparison.CurrentCultureIgnoreCase));

            if (!string.IsNullOrEmpty(resourceName))
            {
                streamAtlas = assemblyToUse.GetManifestResourceStream(resourceName);
            }

            return (streamJson, streamAtlas);
        }

        private (Stream, Stream) GetStreamsFromFile(string nameJson, string nameAtlas)
        {
            Stream streamJson;
            Stream streamAtlas;

            try
            {
                streamJson = File.OpenRead(nameJson);
                streamAtlas = File.OpenRead(nameAtlas);
            }
            catch (Exception)
            {
                return (null, null);
            }

            return (streamJson, streamAtlas);
        }

        private (Stream, Stream) GetStreamsFromUrl(string nameJson, string nameAtlas)
        {
            var streamJson = new MemoryStream();

            // Could be a http or https source
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(nameJson);
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

                resp.GetResponseStream().CopyTo(streamJson);
            }
            catch (Exception)
            {
                return (null, null);
            }

            var streamAtlas = new MemoryStream();

            // Could be a http or https source
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(nameAtlas);
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

                resp.GetResponseStream().CopyTo(streamAtlas);
            }
            catch (Exception)
            {
                return (null, null);
            }

            return (streamJson, streamAtlas);
        }
    }
}
