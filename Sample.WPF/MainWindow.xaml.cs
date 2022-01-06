using BruTile.MbTiles;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers.Tiling;
using Mapsui.Rendering.Skia.SkiaWidgets;
using Mapsui.Utilities;
using Mapsui.Widgets;
using Mapsui.Widgets.PerformanceWidget;
using Mapsui.Widgets.ScaleBar;
using Mapsui.VectorTileLayer.Core;
using SkiaSharp;
using System.Windows;
using SQLite;
using System.IO;
using Mapsui.Layers;
using Mapsui.VectorTileLayer.MapboxGL;
using System.Reflection;
using System.Linq;
using Mapsui.VectorTileLayer.Core.Styles;
using Mapsui.VectorTileLayer.Core.Renderer;
using Mapsui.VectorTileLayer.Core.Extensions;
using Mapsui.Logging;
using System.Collections.Generic;

namespace Sample.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string MbTilesLocation { get; set; } = @"." + Path.DirectorySeparatorChar + "MbTiles";

        private readonly Performance _performance = new Performance(10);

        public MainWindow()
        {
            InitializeComponent();

            Logger.LogDelegate = (level, text, exception) => System.Diagnostics.Debug.WriteLine($"{level}: {text}, {exception}");

            var map = new Map
            {
                CRS = "EPSG:3857"
            };

            //map.Layers.Add(OpenStreetMap.CreateTileLayer());

            // Add ScaleBarWidget
            map.Widgets.Add(new ScaleBarWidget(map)
            {
                TextAlignment = Alignment.Center,
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

            LoadFontResources(Assembly.GetAssembly(GetType()));
            LoadMapboxGL(Assembly.GetAssembly(GetType()));

            mapControl.Navigator.CenterOn(825890.75, 5423194.65);
            mapControl.Navigator.ZoomTo(1.2);
        }

        public void LoadMapboxGL(Assembly assemblyToUse)
        {
            var filename = "monaco.mbtiles";
            MGLStyleLoader.DirectoryForFiles = ".\\MbTiles";

            CheckForMBTilesFile(filename, MGLStyleLoader.DirectoryForFiles);

            // Get Mapbox GL Style File
            var mglStyleFile = CreateMGLStyleFile(assemblyToUse);

            if (mglStyleFile == null)
                return;

            // Ok, we have a valid style file, so get the tile layers, contained in style file
            foreach (var tileLayer in mglStyleFile.TileLayers)
            {
                switch (tileLayer.Style)
                {
                    case BackgroundTileStyle backgroundTileStyle:
                        mapControl.Map.BackColor = new Mapsui.Styles.Color(239, 239, 239);
                        break;
                    case RasterTileStyle rasterTileStyle:
                        break;
                    case VectorTileStyle vectorTileStyle:
                        break;
                }

                tileLayer.MinVisible = 30.ToResolution();
                tileLayer.MaxVisible = 0.ToResolution();

                mapControl.Map.Layers.Add(tileLayer);

                //if (tileSource is MGLRasterTileSource)
                //{
                //    var layer = new TileLayer(tileSource, fetchStrategy: new FetchStrategy(3), fetchToFeature: DrawableTile.DrawableTileToFeature, fetchGetTile: tileSource.GetVectorTile);
                //    layer.MinVisible = tileSource.Schema.Resolutions.Last().Value.UnitsPerPixel;
                //    layer.MaxVisible = tileSource.Schema.Resolutions.First().Value.UnitsPerPixel;
                //    layer.Style = new DrawableTileStyle();
                //    map.Layers.Add(layer);
                //}
            }
        }

        private static string CheckForMBTilesFile(string filename, string dataDir)
        {
            filename = Path.Combine(dataDir, filename);
            if (!File.Exists(filename))
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceNames = assembly.GetManifestResourceNames();
                var resourceName = resourceNames.FirstOrDefault(s => s.ToLower().EndsWith(filename) == true);
                if (resourceName != null)
                {
                    var stream = assembly.GetManifestResourceStream(resourceName);
                    using (var file = new FileStream(filename, FileMode.Create, FileAccess.Write))
                    {
                        stream.CopyTo(file);
                    }
                }
            }

            return filename;
        }

        public MGLStyleFile CreateMGLStyleFile(Assembly assemblyToUse)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assemblyToUse.GetManifestResourceNames();
            var resourceName = resourceNames.FirstOrDefault(s => s.ToLower().EndsWith("styles.osm-liberty.json") == true);

            MGLStyleFile result;

            if (string.IsNullOrEmpty(resourceName))
                return null;

            // Open JSON style files and read contents
            using (var stream = assemblyToUse.GetManifestResourceStream(resourceName))
            {
                // Open JSON style files and read contents
                result = MGLStyleLoader.Load(stream, assemblyToUse);
            }

            return result;
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
                        MGLSymbolStyler.SpecialFonts.Add(resourceName, typeface);
                    }
                }
            }
        }
    }
}
