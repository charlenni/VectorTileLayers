using Mapsui;
using Mapsui.Extensions;
using Mapsui.Tiling;
using Mapsui.Logging;
using Mapsui.Rendering.Skia.SkiaWidgets;
using Mapsui.Utilities;
using Mapsui.VectorTileLayers.Core.Enums;
using Mapsui.VectorTileLayers.Core.Extensions;
using Mapsui.VectorTileLayers.Core.Renderer;
using Mapsui.VectorTileLayers.Core.Styles;
using Mapsui.VectorTileLayers.OpenMapTiles;
using Mapsui.Widgets.PerformanceWidget;
using Mapsui.Widgets.ScaleBar;
using SkiaSharp;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Sample.Forms
{
    public partial class MainPage : ContentPage
    {
        public static string MbTilesLocation { get; set; } = @"." + Path.DirectorySeparatorChar + "MbTiles";

        private readonly Performance _performance = new Performance(10);

        public MainPage()
        {
            InitializeComponent();

            Logger.LogDelegate = (level, text, exception) =>
            {
                if (level == LogLevel.Information)
                    return;

                System.Diagnostics.Debug.WriteLine($"{level}: {text}, {exception}");
            };

            var map = new Mapsui.Map
            {
                CRS = "EPSG:3857"
            };

            //map.Layers.Add(OpenStreetMap.CreateTileLayer());

            // Add ScaleBarWidget
            map.Widgets.Add(new ScaleBarWidget(map)
            {
                TextAlignment = Mapsui.Widgets.Alignment.Center,
                HorizontalAlignment = Mapsui.Widgets.HorizontalAlignment.Center,
                VerticalAlignment = Mapsui.Widgets.VerticalAlignment.Top
            });

            // Add PerformanceWidget
            map.Widgets.Add(new PerformanceWidget(_performance));
            mapControl.Performance = _performance;
            mapControl.Renderer.WidgetRenders[typeof(PerformanceWidget)] = new PerformanceWidgetRenderer(10, 10, 12, SKColors.Black, SKColors.White);

            mapControl.Renderer.StyleRenderers[typeof(BackgroundTileStyle)] = new BackgroundTileStyleRenderer();
            mapControl.Renderer.StyleRenderers[typeof(RasterTileStyle)] = new RasterTileStyleRenderer();
            mapControl.Renderer.StyleRenderers[typeof(VectorTileStyle)] = new VectorTileStyleRenderer();

            mapControl.Map = map;

            Topten.RichTextKit.FontMapper.Default = new Mapsui.VectorTileLayers.OpenMapTiles.Utilities.FontMapper();

            LoadFontResources(Assembly.GetAssembly(GetType()));
            LoadMapboxGL();

            mapControl.Navigator.CenterOn(825890.75, 5423194.65);
            mapControl.Navigator.ZoomTo(17.ToResolution()); // 1.2);
            mapControl.Navigator.RotateTo(0);
        }

        public void LoadMapboxGL()
        {
            var filename = "monaco.mbtiles";
            OMTStyleFileLoader.DirectoryForFiles = FileSystem.AppDataDirectory;

            CheckForMBTilesFile(filename, OMTStyleFileLoader.DirectoryForFiles);

            var stream = EmbeddedResourceLoader.Load("styles.osm-liberty.json", GetType()) ?? throw new FileNotFoundException($"styles.osm - liberty.json not found");

            var layers = new OpenMapTilesLayer(stream, GetLocalContent);

            foreach (var layer in layers)
                mapControl.Map.Layers.Add(layer);
        }

        private static string CheckForMBTilesFile(string filename, string dataDir)
        {
            if (!File.Exists(Path.Combine(dataDir, filename)))
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceNames = assembly.GetManifestResourceNames();
                var resourceName = resourceNames.FirstOrDefault(s => s.ToLower().EndsWith(filename) == true);
                if (resourceName != null)
                {
                    var stream = assembly.GetManifestResourceStream(resourceName);
                    using (var file = new FileStream(Path.Combine(dataDir, filename), FileMode.Create, FileAccess.Write))
                    {
                        stream.CopyTo(file);
                    }
                }
            }

            if (!File.Exists(Path.Combine(dataDir, filename)))
            {
                throw new FileNotFoundException($"File {filename} not found");
            }

            return filename;
        }

        public Stream GetLocalContent(LocalContentType type, string name)
        {
            switch (type)
            {
                case LocalContentType.File:
                    if (File.Exists(name))
                        return File.OpenRead(name);
                    else
                        return null;
                case LocalContentType.Resource:
                    return EmbeddedResourceLoader.Load(name, GetType());
            }

            return null;
        }

        public void LoadFontResources(Assembly assemblyToUse)
        {
            // Try to load this font from resources
            var resourceNames = assemblyToUse?.GetManifestResourceNames();

            foreach (var resourceName in resourceNames.Where(s => s.EndsWith(".ttf", System.StringComparison.CurrentCultureIgnoreCase)))
            {
                var fontName = resourceName.Substring(0, resourceName.Length - 4);
                fontName = fontName.Substring(fontName.LastIndexOf(".") + 1);

                using (var stream = assemblyToUse.GetManifestResourceStream(resourceName))
                {
                    var typeface = SKFontManager.Default.CreateTypeface(stream);

                    if (typeface != null)
                    {
                        //((Mapsui.VectorTileLayers.OpenMapTiles.Utilities.FontMapper)Topten.RichTextKit.FontMapper.Default).Add(typeface);
                    }
                }
            }
        }
    }
}
