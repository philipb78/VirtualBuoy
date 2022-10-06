using Models.CourseItems;

using System;
using System.Collections.Generic;
using System.Text;

namespace ViewModels.CourseVM
{
    public class CourseMarkVM : BaseViewModel
    {
        public int Id
        {
            get { return m_raceMark.Id; }
        }

        public string Name
        {
            get { return m_raceMark.Name; }
            set
            {
                m_raceMark.Name = value;
                SetProperty();
            }
        }

        public CourseMark Mark
        {
            get { return m_raceMark; }
        }

        private CourseMark m_raceMark;

        public CourseMarkVM(CourseMark raceMark)
        {
            m_raceMark = raceMark;
        }

        public override bool Equals(Object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                CourseMarkVM toCompareVM = (CourseMarkVM)obj;
                if (Id == toCompareVM.Id)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}