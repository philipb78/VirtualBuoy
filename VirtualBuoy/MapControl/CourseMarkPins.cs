using Interfaces;
using Mapsui.UI.Forms;
using Models.CourseItems;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace MapControl
{
    public class CourseMarkPins
    {
        private MapView m_mapView;

        private IDataController m_dataController;

        private int m_currentCourseMarkIndex;

        public CourseMarkPins(MapView mapView)
        {
            m_mapView = mapView;
            m_dataController = DependencyService.Get<IDataController>();
            m_dataController.CourseUpdated += CourseUpdated;
        }

        private void CourseUpdated(object sender, EventArgs e)
        {
            m_currentCourseMarkIndex = m_dataController.ActiveCourse.CurrentCourseMarkIndex;
            UpdatePins();
        }

        public void UpdatePins()
        {
            try
            {
                m_mapView.Pins.Clear();
                foreach (ActiveCourseMark nextMark in m_dataController.ActiveCourse.CourseMarks)
                {
                    if (nextMark.Mark != null)
                    {
                        if (nextMark == m_dataController.ActiveCourse.CourseMarks[m_currentCourseMarkIndex])
                        {
                            Pin pin = new Pin(m_mapView)
                            {
                                Label = nextMark.Mark.Name,
                                Position = new Position(nextMark.Mark.Position.Lat, nextMark.Mark.Position.Lon),
                                Address = nextMark.Mark.Name,
                                Type = PinType.Pin,
                                Color = nextMark.MarkSide == Side.Port ? Color.Red : Color.Green,
                                Transparency = 0.5f,
                                Scale = 0.5f,
                                RotateWithMap = true,
                            };
                            m_mapView.Pins.Add(pin);
                        }
                        else
                        {
                            Pin pin = new Pin(m_mapView)
                            {
                                Label = nextMark.Mark.Name,
                                Position = new Position(nextMark.Mark.Position.Lat, nextMark.Mark.Position.Lon),
                                Address = nextMark.Mark.Name,
                                Type = PinType.Pin,
                                Color = Color.Yellow,
                                Transparency = 0.5f,
                                Scale = 0.5f,
                                RotateWithMap = true,
                            };
                            m_mapView.Pins.Add(pin);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}