using Mapsui;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.UI.Forms;
using Mapsui.Utilities;
using Mapsui.Widgets.Button;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using ViewModels;
using Xamarin.Forms;
using BruTile.MbTiles;
using BruTile.Cache;
using System.IO;
using BruTile.Web;
using BruTile.Predefined;
using SQLite;

namespace MapControl
{
    public class CourseMapView : MapView
    {
        private BoatLocationLayer m_boatLocationLayer;
        private CourseLinesLayer m_courseLinesLayer;
        private CourseMarkPins m_courseMarkPins;

        public static readonly BindableProperty MyLocationProperty = BindableProperty.Create(nameof(MyLocation), typeof(Position), typeof(CourseMapView), default(Position), defaultBindingMode: BindingMode.OneWay, propertyChanged: OnMyPositionChanged);
        public static readonly BindableProperty MySpeedProperty = BindableProperty.Create(nameof(MySpeed), typeof(double), typeof(CourseMapView), default(double), defaultBindingMode: BindingMode.OneWay, propertyChanged: OnMySpeedChanged);
        public static readonly BindableProperty MyHeadingProperty = BindableProperty.Create(nameof(MyHeading), typeof(double), typeof(CourseMapView), default(double), defaultBindingMode: BindingMode.OneWay, propertyChanged: OnMyHeadingChanged);
        public static readonly BindableProperty MBTilesMapPathProperty = BindableProperty.Create(nameof(MBTilesMapPath), typeof(string), typeof(CourseMapView), default(string), defaultBindingMode: BindingMode.OneWay, propertyChanged: OnMBTileMapPropertyChanged);

        public string MBTilesMapPath
        {
            get => (string)GetValue(MBTilesMapPathProperty);
            set => SetValue(MBTilesMapPathProperty, value);
        }

        public Double MyHeading
        {
            get => (Double)GetValue(MyHeadingProperty);
            set => SetValue(MyHeadingProperty, value);
        }

        public Double MySpeed
        {
            get => (Double)GetValue(MySpeedProperty);
            set => SetValue(MySpeedProperty, value);
        }

        public Position MyLocation
        {
            get => (Position)GetValue(MyLocationProperty);
            set => SetValue(MyLocationProperty, value);
        }

        public IPersistentCache<byte[]>? DefaultCache = null;

        private BruTile.Attribution OpenStreetMapAttribution = new BruTile.Attribution(
            "© OpenStreetMap contributors", "https://www.openstreetmap.org/copyright");

        private static void OnMBTileMapPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue != oldValue)
            {
                CourseMapView parentMapView = bindable as CourseMapView;
                parentMapView.SetupMap((string)newValue);
            }
        }

        private static void OnMyPositionChanged(BindableObject bindable, object oldValue, object newValue)
        {
            CourseMapView parentMapView = bindable as CourseMapView;
            Position newPosition = (Position)newValue;
            parentMapView.m_boatLocationLayer.UpdateMyLocation(newPosition);
            parentMapView.MyLocationLayer.UpdateMyLocation(newPosition);
        }

        private static void OnMySpeedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            CourseMapView parentMapView = bindable as CourseMapView;
            Double newSpeed = (Double)newValue;

            parentMapView.m_boatLocationLayer.UpdateMySpeed(newSpeed);
        }

        private static async void OnMyHeadingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            CourseMapView parentMapView = bindable as CourseMapView;
            Double newHeading = (Double)newValue;

            await parentMapView.m_boatLocationLayer.UpdateMyDirection(newHeading, 0);
        }

        public CourseMapView() : base()
        {
            SetupMap(String.Empty);
        }

        private void SetupMap(string mbtilePath)
        {
            m_boatLocationLayer = new BoatLocationLayer(this);
            m_courseLinesLayer = new CourseLinesLayer();
            m_courseMarkPins = new CourseMarkPins(this);

            MyLocationLayer.Enabled = false;

            Map = new Map
            {
                CRS = "EPSG:3857",
                Transformation = new MinimalTransformation()
            };
            Map.RotationLock = true;

            TileLayer osmTileLayer = OpenStreetMap.CreateTileLayer();

            Map.Layers.Insert(0, m_boatLocationLayer);
            //Map.Layers.Insert(0, m_textLayer);
            Map.Layers.Insert(0, m_courseLinesLayer);

            if (!string.IsNullOrWhiteSpace(mbtilePath) && File.Exists(mbtilePath))

            {
                TileLayer customTileLayer = CreateMbTilesLayer(mbtilePath, "Custom Map");
                Map.Layers.Insert(0, customTileLayer);
            }
            Map.Layers.Insert(0, osmTileLayer);

            m_boatLocationLayer.Enabled = true;
            m_courseMarkPins.UpdatePins();
        }

        private TileLayer CreateOpenSeaMapTileLayer(string? userAgent = null)
        {
            userAgent ??= $"user-agent-of-{Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName)}";

            return new TileLayer(CreateOpenSeaMapTileSource(userAgent)) { Name = "OpenSeaMap" };
        }

        private HttpTileSource CreateOpenSeaMapTileSource(string userAgent)
        {
            return new HttpTileSource(new GlobalSphericalMercator(), "http://t1.openseamap.org/seamark/{z}/{x}/{y}.png", name: "OpenSeaMap", attribution: OpenStreetMapAttribution, userAgent: "OpenSeaMap");
        }

        public TileLayer CreateMbTilesLayer(string path, string name)
        {
            try
            {
                MbTilesTileSource mbTilesTileSource = new MbTilesTileSource(new SQLiteConnectionString(path, true));
                TileLayer mbTilesLayer = new TileLayer(mbTilesTileSource) { Name = name };
                return mbTilesLayer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}