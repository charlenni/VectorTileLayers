using ExCSS;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Limiting;
using Mapsui.Logging;
using Mapsui.Rendering.Skia;
using Mapsui.Rendering.Skia.SkiaWidgets;
using Mapsui.Styles;
using Mapsui.Utilities;
using Mapsui.VectorTileLayers.Core.Enums;
using Mapsui.VectorTileLayers.Core.Extensions;
using Mapsui.VectorTileLayers.Core.Renderer;
using Mapsui.VectorTileLayers.Core.Styles;
using Mapsui.VectorTileLayers.OpenMapTiles;
using Mapsui.Widgets.InfoWidgets;
using Mapsui.Widgets.ScaleBar;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
        private readonly OMTVectorTileLayer _vectorTileLayer;

        public MainWindow()
        {
            InitializeComponent();

            Logger.LogDelegate = (level, text, exception) =>
            {
                if (level == LogLevel.Information)
                    return;

                System.Diagnostics.Debug.WriteLine($"{level}: {text}, {exception}");
            };

            var map = new Map
            {
                CRS = "EPSG:3857"
            };

            //map.Layers.Add(OpenStreetMap.CreateTileLayer());

            // Add handler for zoom buttons
            btnZoomIn.Click += BtnZoomIn_Click;
            btnZoomOut.Click += BtnZoomOut_Click;
            btnRotateCW.Click += BtnRotateCW_Click;
            btnRotateCCW.Click += BtnRotateCCW_Click;

            // Add ScaleBarWidget
            map.Widgets.Add(new ScaleBarWidget(map)
            {
                TextAlignment = Mapsui.Widgets.Alignment.Center,
                HorizontalAlignment = Mapsui.Widgets.HorizontalAlignment.Center,
                VerticalAlignment = Mapsui.Widgets.VerticalAlignment.Top
            });

            LoggingWidget.ShowLoggingInMap = Mapsui.Widgets.ActiveMode.No;

            MapRenderer.RegisterStyleRenderer(typeof(BackgroundTileStyle), new BackgroundTileStyleRenderer());
            MapRenderer.RegisterStyleRenderer(typeof(RasterTileStyle), new RasterTileStyleRenderer());
            MapRenderer.RegisterStyleRenderer(typeof(VectorTileStyle), new VectorTileStyleRenderer());

            mapControl.Map = map;

            Topten.RichTextKit.FontMapper.Default = new Mapsui.VectorTileLayers.OpenMapTiles.Utilities.FontMapper();

            LoadFontResources(Assembly.GetAssembly(GetType()));
            LoadMapboxGL();

            ObservableCollection<CheckBoxListViewItem> items = new ObservableCollection<CheckBoxListViewItem>();

            items.CollectionChanged += ListViewCollection_Changed;

            listViewStyles.ItemsSource = items;

            btnAll.Click += BtnAll_Click;
            btnNone.Click += BtnNone_Click;

            // Get vector tile layer for listbox of style layers
            foreach (ILayer layer in mapControl.Map.Layers)
            {
                if (layer is OMTVectorTileLayer)
                    _vectorTileLayer = (OMTVectorTileLayer)layer;
            }

            if (_vectorTileLayer?.Style is VectorTileStyle vectorTileStyle)
            {
                for (int i = 0; i < vectorTileStyle.StyleLayers.Count(); i++)
                {
                    foreach (var style in vectorTileStyle.StyleLayers)
                    {
                        if (style is OMTStyle vectorStyle)
                        {
                            var item = new CheckBoxListViewItem(vectorStyle, vectorStyle.Id, vectorStyle.Enabled);
                            item.PropertyChanged += Item_PropertyChanged;
                            items.Add(item);
                        }
                    }
                }
            }

            // Zurich
            mapControl.Map.Navigator.CenterOn(950804.77, 6002071.45);
            // Monaco
            // mapControl.Navigator.CenterOn(825890.75, 5423194.65);
            mapControl.Map.Navigator.ZoomTo(17.ToResolution()); // 1.2);
            mapControl.Map.Navigator.RotateTo(0);
        }

        private void BtnRotateCCW_Click(object sender, RoutedEventArgs e)
        {
            mapControl.Map.Navigator.RotateTo(mapControl.Map.Navigator.Viewport.Rotation - 5);
        }

        private void BtnRotateCW_Click(object sender, RoutedEventArgs e)
        {
            mapControl.Map.Navigator.RotateTo(mapControl.Map.Navigator.Viewport.Rotation + 5);
        }

        private void BtnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            mapControl.Map.Navigator.ZoomIn(0);
        }

        private void BtnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            mapControl.Map.Navigator.ZoomOut(0);
        }

        private void BtnAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in listViewStyles.ItemsSource)
            {
                ((CheckBoxListViewItem)item).IsChecked = true;
            }
        }

        private void BtnNone_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in listViewStyles.ItemsSource)
            {
                ((CheckBoxListViewItem)item).IsChecked = false;
            }
        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CheckBoxListViewItem item = (CheckBoxListViewItem)sender;

            if (item.Style.Enabled != item.IsChecked)
            {
                item.Style.Enabled = item.IsChecked;
                mapControl.ForceUpdate();
            }
        }

        private void ListViewCollection_Changed(object s, NotifyCollectionChangedEventArgs e)
        {
            foreach (CheckBoxListViewItem item in e.NewItems)
            {
                item.Style.Enabled = item.IsChecked;
                mapControl.ForceUpdate();
            }
        }
        public void LoadMapboxGL()
        {
            var filename = "switzerland_zurich.mbtiles"; // monaco.mbtiles";
            var directory = ".\\mbtiles";

            // Check if demo file exists
            CheckForMBTilesFile(filename, directory);
            // Load embedded resource of style file
            var streamOfStyleFile = EmbeddedResourceLoader.Load("styles.osm-liberty.json", GetType()) ?? throw new FileNotFoundException($"Embedded styles.osm-liberty.json not found");

            // Set values for style file loader
            OMTStyleFileLoader.DirectoryForFiles = ".\\mbtiles";
            OMTStyleFileLoader.GetLocalContent = GetLocalContent;
            // Load style file
            var styleFile = OMTStyleFileLoader.Load(streamOfStyleFile);

            foreach (var tileLayer in styleFile.TileLayers)
            {
                switch (tileLayer.Style)
                {
                    case BackgroundTileStyle backgroundTileStyle:
                        mapControl.Map.Layers.Add(tileLayer);
                        break;
                    case RasterTileStyle rasterTileStyle:
                        mapControl.Map.Layers.Add(tileLayer);
                        break;
                    case VectorTileStyle vectorTileStyle:
                        mapControl.Map.Layers.Add(tileLayer);
                        break;
                }
            }
        }

        /// <summary>
        /// Make sure, that the MBTiles file exists in the directory
        /// </summary>
        /// <param name="filename">Filename of MBTiles file</param>
        /// <param name="directory">Directory for checking</param>
        /// <returns>Combined directory and filename</returns>
        private static string CheckForMBTilesFile(string filename, string directory)
        {
            filename = Path.Combine(directory, filename);
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
                        ((Mapsui.VectorTileLayers.OpenMapTiles.Utilities.FontMapper)Topten.RichTextKit.FontMapper.Default).Add(typeface);
                    }
                }
            }
        }
    }
}
