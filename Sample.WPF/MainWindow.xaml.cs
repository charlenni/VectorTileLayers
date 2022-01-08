using Mapsui;
using Mapsui.Extensions;
using Mapsui.Logging;
using Mapsui.Rendering.Skia.SkiaWidgets;
using Mapsui.Utilities;
using Mapsui.VectorTileLayer.Core.Enums;
using Mapsui.VectorTileLayer.Core.Renderer;
using Mapsui.VectorTileLayer.Core.Styles;
using Mapsui.VectorTileLayer.MapboxGL;
using Mapsui.Widgets.PerformanceWidget;
using Mapsui.Widgets.ScaleBar;
using SkiaSharp;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

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

            LoadFontResources(Assembly.GetAssembly(GetType()));
            LoadMapboxGL();

            mapControl.Navigator.CenterOn(825890.75, 5423194.65);
            mapControl.Navigator.ZoomTo(1.2);
        }

        public void LoadMapboxGL()
        {
            var filename = "monaco.mbtiles";
            MGLStyleLoader.DirectoryForFiles = ".\\mbtiles";

            CheckForMBTilesFile(filename, MGLStyleLoader.DirectoryForFiles);

            var stream = EmbeddedResourceLoader.Load("styles.osm-liberty.json", GetType()) ?? throw new FileNotFoundException($"styles.osm - liberty.json not found");

            var layers = new MapboxGLLayers(stream, GetLocalContent);

            foreach (var layer in layers)
                mapControl.Map.Layers.Add(layer);
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

        public Stream GetLocalContent(LocalContentType type, string name)
        {
            switch (type)
            {
                case LocalContentType.File:
                    if (File.Exists(name))
                        return File.OpenRead(name);
                    else
                        return null;
                    break;
                case LocalContentType.Resource:
                    return EmbeddedResourceLoader.Load(name, GetType());
                    break;
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
                        MGLSymbolStyler.SpecialFonts.Add(resourceName, typeface);
                    }
                }
            }
        }
    }
}
