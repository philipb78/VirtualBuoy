using Interfaces;
using Models.CourseItems;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace ViewModels.CourseVM
{
    public class CurrentCourseMarkVM : BaseViewModel
    {
        private IDataController m_dataController;

        public ICommand PreviousMarkCommand { private set; get; }

        public ICommand NextMarkCommand { private set; get; }

        private string m_markName;

        public string MarkName
        {
            get
            {
                return m_markName;
            }

            set
            {
                m_markName = value;
                SetProperty();
            }
        }

        private string m_distanceToMark;

        public string DistanceToMark
        {
            get
            {
                return m_distanceToMark;
            }
            set
            {
                m_distanceToMark = value;
                SetProperty();
            }
        }

        private string m_timeToMark;

        public string TimeToMark
        {
            get
            {
                return m_timeToMark;
            }

            set
            {
                m_timeToMark = value;
                SetProperty();
            }
        }

        private string m_vmg;

        public string VMG
        {
            get
            {
                return m_vmg;
            }

            set
            {
                m_vmg = value;
                SetProperty();
            }
        }

        private string m_speed;

        public string Speed
        {
            get
            {
                return m_speed;
            }
            set
            {
                m_speed = value;
                SetProperty();
            }
        }

        private Color m_markColor;

        public Color MarkColor
        {
            get
            {
                return m_markColor;
            }
            set
            {
                m_markColor = value;
                SetProperty();
            }
        }

        public CurrentCourseMarkVM()
        {
            m_dataController = DependencyService.Get<IDataController>();
            m_dataController.DataUpdated += DataController_DataUpdated;
            m_dataController.CourseUpdated += DataController_CourseUpdated;
            DataController_DataUpdated(null, null);

            NextMarkCommand = new Command(NextMark, CanExecuteNextMark);
            PreviousMarkCommand = new Command(PreviousMark, CanExecutePreviousMark);
        }

        private void DataController_CourseUpdated(object sender, EventArgs e)
        {
            (NextMarkCommand as Command).ChangeCanExecute();
            (PreviousMarkCommand as Command).ChangeCanExecute();
            DataController_DataUpdated(null, null);
        }

        private bool CanExecutePreviousMark(object arg)
        {
            if (m_dataController.ActiveCourse.CurrentCourseMarkIndex > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void PreviousMark(object obj)
        {
            m_dataController.SetLastMarkActive(this);
        }

        private bool CanExecuteNextMark(object arg)
        {
            if (m_dataController.ActiveCourse.CurrentCourseMarkIndex < m_dataController.ActiveCourse.CourseMarks.Count - 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void NextMark(object obj)
        {
            m_dataController.SetNextMarkActive(this);
        }

        private void DataController_DataUpdated(object sender, EventArgs e)
        {
            try
            {
                CourseMarkData raceMarkData = m_dataController.ActiveCourse.CurrentCourseMarkData;
                if (m_dataController.ActiveCourse.CurrentCourseMarkIndex < m_dataController.ActiveCourse.CourseMarks.Count && m_dataController.ActiveCourse.CourseMarks[m_dataController.ActiveCourse.CurrentCourseMarkIndex].Mark != null)
                {
                    MarkName = m_dataController.ActiveCourse.CourseMarks[m_dataController.ActiveCourse.CurrentCourseMarkIndex].Mark.Name;
                    MarkColor = m_dataController.ActiveCourse.CourseMarks[m_dataController.ActiveCourse.CurrentCourseMarkIndex].MarkSide == Side.Port ? Color.Red : Color.Green;
                }
                else
                {
                    MarkName = "No Mark";
                }
                if (raceMarkData != null && raceMarkData.DataCalculated)
                {
                    DistanceToMark = string.Format("{0:0.00} M", raceMarkData.DistanceToMark);
                    TimeToMark = raceMarkData.TimeToMark.ToString("mm\\:ss");
                    Speed = string.Format("{0:0.00} Kn", (m_dataController.BoatData.SOG * 1.943844));
                    VMG = string.Format("{0:0.00} Kn", raceMarkData.Vmg);
                }
                else
                {
                    DistanceToMark = "No Data";
                    TimeToMark = "No Data";
                    Speed = "No Data";
                    VMG = "No Data";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}