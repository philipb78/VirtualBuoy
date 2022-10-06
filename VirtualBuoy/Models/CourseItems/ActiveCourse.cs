using System;
using System.Collections.Generic;
using System.Text;

namespace Models.CourseItems
{
    public class ActiveCourse
    {
        public List<ActiveCourseMark> CourseMarks { get; set; }


        /// <summary>
        /// Current Race Mark Index
        /// </summary>
        public int CurrentCourseMarkIndex { get; set; }
    }
}