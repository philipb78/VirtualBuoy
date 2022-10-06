using Models.CourseItems;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ViewModels.CourseVM
{
    public class ActiveCourseMarkVM : BaseViewModel
    {
        private ActiveCourseMark m_activeCourseMark;

        public ActiveCourseMark ActiveCourseMark
        {
            get { return m_activeCourseMark; }
        }

        private CourseMarkVM m_courseMark;

        public CourseMarkVM CourseMark
        {
            get
            {
                return m_courseMark;
            }

            set
            {
                m_courseMark = value;
                if (value != null)
                {
                    m_activeCourseMark.Mark = value.Mark;
                }

                SetProperty();
            }
        }

        public int Order
        {
            get { return m_activeCourseMark.Order; }
            set
            {
                m_activeCourseMark.Order = value;
                SetProperty();
            }
        }

        public bool IsStarboardPass
        {
            get
            {
                if (m_activeCourseMark.MarkSide == Side.Starboard)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (value)
                {
                    m_activeCourseMark.MarkSide = Side.Starboard;
                }
                else
                {
                    m_activeCourseMark.MarkSide = Side.Port;
                }
                UpdateMarkSide();

                SetProperty();
            }
        }

        private string m_markSide;

        public string MarkSide
        {
            get { return m_markSide; }
            set
            {
                m_markSide = value;
                SetProperty();
            }
        }

        private Color m_markSideColour;

        public Color MarkSideColour
        {
            get { return m_markSideColour; }
            set
            {
                m_markSideColour = value;
                SetProperty();
            }
        }

        public void UpdateMarkSide()
        {
            MarkSide = m_activeCourseMark.MarkSide.ToString();
            if (m_activeCourseMark.MarkSide == Side.Port)
            {
                MarkSideColour = Color.Red;
            }
            else
            {
                MarkSideColour = Color.Green;
            }
        }

        public ActiveCourseMarkVM(ActiveCourseMark activeCourseMark)
        {
            m_activeCourseMark = activeCourseMark;

            UpdateMarkSide();
        }

        public ActiveCourseMarkVM()
        {
            m_activeCourseMark = new ActiveCourseMark();
            UpdateMarkSide();
        }
    }
}