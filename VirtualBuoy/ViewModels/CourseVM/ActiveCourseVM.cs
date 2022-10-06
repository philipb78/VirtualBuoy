using Models.CourseItems;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ViewModels.CourseVM
{
    public class ActiveCourseVM
    {
        private ActiveCourse ActiveCourse { get; set; }
        public ObservableCollection<ActiveCourseMarkVM> ActiveCourseMarks { get; set; }

        public ActiveCourseVM()
        {
        }
    }
}