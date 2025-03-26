using Mapsui.Styles;
using Mapsui.VectorTileLayers.Core.Enums;
using Mapsui.VectorTileLayers.Core.Interfaces;
using Mapsui.VectorTileLayers.Core.Primitives;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Mapsui.VectorTileLayers.OpenMapTiles
{
    public class OMTSpriteAtlas : ISpriteAtlas
    {
        Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

        /// <summary>
        /// Add a OpenMapTiles source to atlas
        /// </summary>
        /// <param name="source">Url or path name to get sprites definition and atlas from</param>
        public void AddSpriteSource(string source, Func<LocalContentType, string, Stream> getLocalContent)
        {
            Stream streamJson = null;
            
            // TODO: Remove this, if PixelRatio is respected in Mapsui
            // First check for @2x
            var jsonSource = source + ".json"; // "@2x.json";
            // TODO: !!! Change back. Its only because of using ImageFetcher
            var imageSource = source.Replace("styles", "sample/wpf/styles").Replace("/", ".").Replace("embedded:..", "embedded://") + ".png"; // "@2x.png";

            try
            {
                streamJson = GetStreams(jsonSource, getLocalContent);

                if (streamJson == null)
                {
                    // One of them could be != null
                    streamJson?.Dispose();

                    // Perhaps there are no @2x versions
                    jsonSource = source + ".json";
                    imageSource = source + ".png";

                    streamJson = GetStreams(jsonSource, getLocalContent);
                }

                if (streamJson != null)
                {
                    CreateSprites(imageSource, streamJson);
                }
            }
            finally
            {
                streamJson?.Dispose();
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
        public void CreateSprites(string imageSource, Stream jsonStream)
        {
            string json;

            using (var reader = new StreamReader(jsonStream))
            {
                json = reader.ReadToEnd();
            }

            CreateSprites(imageSource, json);
        }

        /// <summary>
        /// Create sprite atlas from a string
        /// </summary>
        /// <param name="json">String with Json sprite file information</param>
        /// <param name="atlas">SKImage of sprite atlas</param>
        private async void CreateSprites(string imageSource, string json)
        {
            // If there isn't a ImageSource, then don't extract any sprites
            if (string.IsNullOrEmpty(imageSource))
                return;

            var sprites = JsonConvert.DeserializeObject<Dictionary<string, Json.JsonSprite>>(json);
            var atlasData = await ImageFetcher.FetchBytesFromImageSourceAsync(imageSource);
            var atlasImage = SKImage.FromEncodedData(SKData.CreateCopy(atlasData));

            foreach (var sprite in sprites)
            {
                // Extract sprite from atlas
                AddSprite(sprite.Key, new OMTSprite(atlasImage, sprite));
            }
        }

        private Stream GetStreams(string nameJson, Func<LocalContentType, string, Stream> getLocalContent)
        {
            if (nameJson.StartsWith("http"))
            {
                return GetStreamFromUrl(nameJson);
            }
            else if (nameJson.StartsWith("file://"))
            {
                nameJson = nameJson.Substring(7);

                return GetStreamFromFile(nameJson, getLocalContent);
            }
            else if (nameJson.StartsWith("embedded://"))
            {
                nameJson = nameJson.Substring(11).Replace('/', '.');

                return GetStreamFromResource(nameJson, getLocalContent);
            }

            // Unknown source type, so do nothing
            throw new NotImplementedException("Unknown URL for sprite");
        }

        private static Stream GetStreamFromResource(string nameJson, Func<LocalContentType, string, Stream> getLocalContent)
        {
            if (getLocalContent == null)
                throw new ArgumentException($"{nameof(getLocalContent)} should not be null");

            return getLocalContent(LocalContentType.Resource, nameJson);
        }

        private static Stream GetStreamFromFile(string nameJson, Func<LocalContentType, string, Stream> getLocalContent)
        {
            Stream streamJson;

            if (getLocalContent == null)
                throw new ArgumentNullException($"{nameof(getLocalContent)} must not be null");

            try
            {
                streamJson = getLocalContent(LocalContentType.File, nameJson);

                if (streamJson == null)
                    throw new FileNotFoundException($"File '{nameJson}' not found");
            }
            catch (Exception)
            {
                return null;
            }

            return streamJson;
        }

        private static Stream GetStreamFromUrl(string nameJson)
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
                return null;
            }

            return streamJson;
        }
    }
}
