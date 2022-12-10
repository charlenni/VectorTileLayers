using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Logging;
using Mapsui.Rendering.Skia.SkiaWidgets;
using Mapsui.Styles;
using Mapsui.Utilities;
using Mapsui.VectorTileLayers.Core.Enums;
using Mapsui.VectorTileLayers.Core.Extensions;
using Mapsui.VectorTileLayers.Core.Renderer;
using Mapsui.VectorTileLayers.Core.Styles;
using Mapsui.VectorTileLayers.OpenMapTiles;
using Mapsui.Widgets.PerformanceWidget;
using Mapsui.Widgets.ScaleBar;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

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

            // Add PerformanceWidget
            map.Widgets.Add(new PerformanceWidget(_performance));
            mapControl.Performance = _performance;
            mapControl.Renderer.WidgetRenders[typeof(PerformanceWidget)] = new PerformanceWidgetRenderer(10, 10, 12, SKColors.Black, SKColors.White);

            mapControl.Renderer.StyleRenderers[typeof(BackgroundTileStyle)] = new BackgroundTileStyleRenderer();
            mapControl.Renderer.StyleRenderers[typeof(RasterTileStyle)] = new RasterTileStyleRenderer();
            mapControl.Renderer.StyleRenderers[typeof(VectorTileStyle)] = new VectorTileStyleRenderer();
            mapControl.Renderer.StyleRenderers[typeof(SymbolTileStyle)] = new SymbolTileStyleRenderer();

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

            if (_vectorTileLayer?.Style is StyleCollection)
            {
                for (int i = 0; i < ((StyleCollection)_vectorTileLayer.Style).Count; i++)
                {
                    if (((StyleCollection)_vectorTileLayer.Style)[i] is VectorTileStyle vts)
                    {
                        foreach (var style in vts.VectorTileStyles)
                        {
                            if (style is OMTVectorTileStyle vectorStyle)
                            {
                                var item = new CheckBoxListViewItem(vectorStyle, vectorStyle.Id, vectorStyle.Enabled);
                                item.PropertyChanged += Item_PropertyChanged;
                                items.Add(item);
                            }
                        }
                    }
                    //if (((StyleCollection)_vectorTileLayer.Style)[i] is SymbolTileStyle sts)
                    //{
                    //    foreach (var style in sts.VectorTileStyles)
                    //    {
                    //        if (style is OMTVectorTileStyle symbolStyle)
                    //            items.Add(new CheckBoxListViewItem(symbolStyle, symbolStyle.Id, symbolStyle.Enabled));
                    //    }
                    //}
                }
            }

            // Zurich
            mapControl.Navigator.CenterOn(950804.77, 6002071.45);
            // Monaco
            // mapControl.Navigator.CenterOn(825890.75, 5423194.65);
            mapControl.Navigator.ZoomTo(17.ToResolution()); // 1.2);
            mapControl.Navigator.RotateTo(0);
        }

        private void BtnRotateCCW_Click(object sender, RoutedEventArgs e)
        {
            mapControl.Navigator.RotateTo(mapControl.Viewport.Rotation - 5);
        }

        private void BtnRotateCW_Click(object sender, RoutedEventArgs e)
        {
            mapControl.Navigator.RotateTo(mapControl.Viewport.Rotation + 5);
        }

        private void BtnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            mapControl.Navigator.ZoomIn(0);
        }

        private void BtnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            mapControl.Navigator.ZoomOut(0);
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

            OMTStyleFileLoader.DirectoryForFiles = ".\\mbtiles";

            CheckForMBTilesFile(filename, OMTStyleFileLoader.DirectoryForFiles);

            var stream = EmbeddedResourceLoader.Load("styles.osm-liberty.json", GetType()) ?? throw new FileNotFoundException($"styles.osm - liberty.json not found");

            var layers = new OpenMapTilesLayer(stream, GetLocalContent);

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
