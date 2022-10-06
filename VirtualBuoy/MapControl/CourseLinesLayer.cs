using Interfaces;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.UI.Forms;
using Models.CourseItems;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MapControl
{
    public class CourseLinesLayer : MemoryLayer
    {
        private MemoryProvider m_provider;

        private Feature m_courseLine;

        private IDataController m_dataController;

        public CourseLinesLayer() : base()
        {
            Name = "Course Line Layer";

            m_provider = new MemoryProvider();
            DataSource = m_provider;

            m_dataController = DependencyService.Get<IDataController>();

            m_dataController.DataUpdated += DataController_DataUpdated;

            m_dataController.CourseUpdated += DataController_CourseUpdated;
        }

        private void DataController_CourseUpdated(object sender, EventArgs e)
        {
            DataController_DataUpdated(this, null);
        }

        private void DataController_DataUpdated(object sender, EventArgs e)
        {
            // Update Raceline
            RedrawLayer();
        }

        private void RaceStateChanged()
        {
            RedrawLayer();
        }

        private void UpdateRaceLine()
        {
            Position lineStartPosition = new Position(m_dataController.BoatData.Lat, m_dataController.BoatData.Lon);

            Position lineEndPositon = new Position(m_dataController.ActiveCourse.CourseMarks[m_dataController.ActiveCourse.CurrentCourseMarkIndex].Mark.Position.Lat, m_dataController.ActiveCourse.CourseMarks[m_dataController.ActiveCourse.CurrentCourseMarkIndex].Mark.Position.Lon);

            List<Mapsui.Geometries.Point> raceLinePositions = new List<Mapsui.Geometries.Point>();
            raceLinePositions.Add(lineStartPosition.ToMapsui());
            raceLinePositions.Add(lineEndPositon.ToMapsui());
            m_courseLine.Geometry = new LineString(raceLinePositions);
        }

        public void RedrawLayer()
        {
            if (m_dataController.ActiveCourse.CourseMarks.Count > 0 && m_dataController.ActiveCourse.CourseMarks[m_dataController.ActiveCourse.CurrentCourseMarkIndex].Mark != null)
            {
                List<Feature> features = new List<Feature>();

                m_courseLine = new Feature()
                {
                    ["Label"] = "Course Line"
                };

                m_courseLine.Styles.Clear();
                ActiveCourseMark nextMark = m_dataController.ActiveCourse.CourseMarks[m_dataController.ActiveCourse.CurrentCourseMarkIndex];
                Mapsui.Styles.Color lineColor = nextMark.MarkSide == Side.Port ? Mapsui.Styles.Color.Red : Mapsui.Styles.Color.Green;

                m_courseLine.Styles.Add(new VectorStyle
                {
                    Line = new Pen { Width = 2, Color = lineColor }
                });
                UpdateRaceLine();

                features.Add(m_courseLine);

                m_provider.ReplaceFeatures(features);
            }
        }
    }
}