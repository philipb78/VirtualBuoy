using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.UI.Forms;
using Mapsui.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;
using Geo;
using Mapsui.Geometries;
using Mapsui.UI.Forms.Extensions;
using Interfaces;
using Xamarin.Forms;
using Models.Admin;
using System.Threading.Tasks;
using Color = Mapsui.Styles.Color;

namespace MapControl
{
    public class BoatLocationLayer : MemoryLayer
    {
        private MapView m_mapView;

        private bool m_isMoving = false;

        public bool IsMoving
        {
            get { return m_isMoving; }
            set { m_isMoving = value; }
        }

        private static int bitmapBoat = -1;

        private Feature m_boatFeature;

        private Feature m_frontLineFeature;

        private Feature m_starboardLineFeature;

        private Feature m_portLineFeature;

        private Position m_myLocation = new Position(0, 0);

        private Position m_forwardLineEnd = new Position(0, 0);

        private Position m_portTackLineEnd = new Position(0, 0);

        private Position m_starboardTackLineEnd = new Position(0, 0);

        private GeoLib geoLib = new GeoLib();

        /// <summary>
        /// Position of location, that is displayed
        /// </summary>
        /// <value>Position of location</value>
        public Position MyLocation => m_myLocation;

        /// <summary>
        /// Movement direction of device at location
        /// </summary>
        /// <value>Direction at location</value>
        public double Direction { get; private set; } = 0.0;

        /// <summary>
        /// Viewing direction of device (in degrees wrt. north direction)
        /// </summary>
        /// <value>Direction at location</value>
        public double ViewingDirection { get; private set; } = -1.0;

        /// <summary>
        /// Scale of symbol
        /// </summary>
        /// <value>Scale of symbol</value>
        public double Scale { get; set; } = 1.0;

        public BoatLocationLayer(MapView mapView)
           : base()
        {
            try
            {
                Style = null;
                m_mapView = mapView;

                if (bitmapBoat == -1)
                {
                    Stream bitmapMoving = EmbeddedResourceLoader.Load("Images.boaticon.png", typeof(BoatLocationLayer));

                    if (bitmapMoving != null)
                    {
                        // Register bitmap
                        bitmapBoat = BitmapRegistry.Instance.Register(bitmapMoving);
                    }
                }

                m_boatFeature = new Feature
                {
                    Geometry = m_myLocation.ToMapsui(),
                    ["Label"] = "My Boat Image"
                };

                m_frontLineFeature = new Feature()
                {
                    ["Label"] = "My Boat Front Line"
                };
                m_frontLineFeature.Styles.Clear();
                m_frontLineFeature.Styles.Add(new VectorStyle
                {
                    Line = new Pen { Width = 2, Color = Color.Orange }
                });

                m_portLineFeature = new Feature()
                {
                    ["Label"] = "My Boat Port Line"
                };

                m_portLineFeature.Styles.Clear();
                m_portLineFeature.Styles.Add(new VectorStyle
                {
                    Line = new Pen { Width = 2, Color = Color.Orange }
                });
                m_starboardLineFeature = new Feature()
                {
                    ["Label"] = "My Boat Starboard Line"
                };
                m_starboardLineFeature.Styles.Clear();
                m_starboardLineFeature.Styles.Add(new VectorStyle
                {
                    Line = new Pen { Width = 2, Color = Color.Orange }
                });
                List<Feature> features = new List<Feature>();
                features.Add(m_boatFeature);
                features.Add(m_frontLineFeature);
                features.Add(m_portLineFeature);
                features.Add(m_starboardLineFeature);
                UpdateLines();
                UpdateBoatImage();
                DataSource = new MemoryProvider(features);
            }
            catch (Exception ex)
            {
            }
        }

        private void UpdateBoatImage()
        {
            m_boatFeature.Styles.Clear();
            m_boatFeature.Styles.Add(new SymbolStyle
            {
                Enabled = true,
                BitmapId = bitmapBoat,

                SymbolScale = 0.75,
                SymbolRotation = 0,
                Opacity = 1,
                SymbolOffset = new Offset(0, 0),
            });
        }

        private async Task UpdateLines()
        {
            ISettingsService settingsService = DependencyService.Get<ISettingsService>();
            Settings settings = await settingsService.GetActiveSettings();
            Models.Point forwardLineEndPoint = geoLib.DestinationPoint(new Models.Point(m_myLocation.Latitude, m_myLocation.Longitude), 100000, Direction);
            m_forwardLineEnd = new Position(forwardLineEndPoint.Lat, forwardLineEndPoint.Lon);
            List<Mapsui.Geometries.Point> frontLinePositions = new List<Mapsui.Geometries.Point>();
            frontLinePositions.Add(m_myLocation.ToMapsui());
            frontLinePositions.Add(m_forwardLineEnd.ToMapsui());
            m_frontLineFeature.Geometry = new LineString(frontLinePositions);

            double portAngle = Direction - settings.TackAngle;
            if (portAngle < 0)
            {
                portAngle = 360 + portAngle;
            }
            Models.Point portLineEndPoint = geoLib.DestinationPoint(new Models.Point(m_myLocation.Latitude, m_myLocation.Longitude), 100000, portAngle);
            m_portTackLineEnd = new Position(portLineEndPoint.Lat, portLineEndPoint.Lon);
            List<Mapsui.Geometries.Point> portLinePositions = new List<Mapsui.Geometries.Point>();
            portLinePositions.Add(m_myLocation.ToMapsui());
            portLinePositions.Add(m_portTackLineEnd.ToMapsui());
            m_portLineFeature.Geometry = new LineString(portLinePositions);

            double starboardAngle = Direction + settings.TackAngle;
            if (starboardAngle > 360)
            {
                starboardAngle = starboardAngle - 360;
            }

            Models.Point starboardLineEndPoint = geoLib.DestinationPoint(new Models.Point(m_myLocation.Latitude, m_myLocation.Longitude), 100000, starboardAngle);
            m_starboardTackLineEnd = new Position(starboardLineEndPoint.Lat, starboardLineEndPoint.Lon);
            List<Mapsui.Geometries.Point> starboardLinePositions = new List<Mapsui.Geometries.Point>();
            starboardLinePositions.Add(m_myLocation.ToMapsui());
            starboardLinePositions.Add(m_starboardTackLineEnd.ToMapsui());
            m_starboardLineFeature.Geometry = new LineString(starboardLinePositions);
        }

        /// <summary>
        /// Updates my location
        /// </summary>
        /// <param name="newLocation">New location</param>
        public void UpdateMyLocation(Position newLocation, bool animated = false)
        {
            if (!MyLocation.Equals(newLocation))
            {
                bool modified = InternalUpdateMyLocation(newLocation);
                // Update viewport
                if (modified && m_mapView.MyLocationFollow)
                    m_mapView.Navigator.CenterOn(MyLocation.ToMapsui());
                // Refresh map

                m_mapView.Refresh();
            }
        }

        /// <summary>
        /// Updates my movement direction
        /// </summary>
        /// <param name="newDirection">New direction</param>
        /// <param name="newViewportRotation">New viewport rotation</param>
        public async Task UpdateMyDirection(double newDirection, double newViewportRotation, bool animated = false)
        {
            var newRotation = (int)(newDirection - newViewportRotation);
            var oldRotation = (int)((SymbolStyle)m_boatFeature.Styles.First()).SymbolRotation;

            if (newRotation != oldRotation)
            {
                Direction = newDirection;

                if (newRotation < 90 && oldRotation > 270)
                {
                    newRotation += 360;
                }
                else if (newRotation > 270 && oldRotation < 90)
                {
                    oldRotation += 360;
                }

                    ((SymbolStyle)m_boatFeature.Styles.First()).SymbolRotation = newRotation % 360;

                await UpdateLines();
                m_mapView.Refresh();
            }
        }

        /// <summary>
        /// Updates my speed
        /// </summary>
        /// <param name="newSpeed">New speed</param>
        public void UpdateMySpeed(double newSpeed)
        {
            var modified = false;

            if (newSpeed > 0 && !IsMoving)
            {
                IsMoving = true;
                modified = true;
            }

            if (newSpeed <= 0 && IsMoving)
            {
                IsMoving = false;
                modified = true;
            }

            if (modified)
                m_mapView.Refresh();
        }

        /// <summary>
        /// Updates my view direction
        /// </summary>
        /// <param name="newDirection">New direction</param>
        /// <param name="newViewportRotation">New viewport rotation</param>
        public void UpdateMyViewDirection(double newDirection, double newViewportRotation, bool animated = false)
        {
            int newRotation = (int)(newDirection - newViewportRotation);
            int oldRotation = (int)((SymbolStyle)m_boatFeature.Styles.First()).SymbolRotation;

            if (newRotation == -1.0)
            {
                // disable bitmap
                ((SymbolStyle)m_boatFeature.Styles.First()).Enabled = false;
            }
            else if (newRotation != oldRotation)
            {
                ((SymbolStyle)m_boatFeature.Styles.First()).Enabled = true;
                ViewingDirection = newDirection;

                if (newRotation < 90 && oldRotation > 270)
                {
                    newRotation += 360;
                }
                else if (newRotation > 270 && oldRotation < 90)
                {
                    oldRotation += 360;
                }

                    ((SymbolStyle)m_boatFeature.Styles.First()).SymbolRotation = newRotation % 360;
                m_mapView.Refresh();
            }
        }

        private bool InternalUpdateMyLocation(Position newLocation)
        {
            bool modified = false;

            if (!m_myLocation.Equals(newLocation))
            {
                m_myLocation = newLocation;

                m_boatFeature.Geometry = m_myLocation.ToMapsui();
                UpdateLines();
                modified = true;
            }

            return modified;
        }
    }
}