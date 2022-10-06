using System;
using System.Collections.Generic;
using System.Text;

namespace Models.CourseItems
{
    public enum Side
    {
        Port,
        Starboard
    }

    public class ActiveCourseMark
    {
        public ActiveCourseMark()
        {
        }

        public CourseMark Mark { get; set; }

        public Side MarkSide { get; set; }

        public int Order { get; set; }

        public ActiveCourseMark Clone()
        {
            ActiveCourseMark activeCourse = new ActiveCourseMark();
            if (Mark != null)
            {
                CourseMark courseMark = new CourseMark();
                courseMark.Id = Mark.Id;
                courseMark.Name = Mark.Name;
                courseMark.Position = Mark.Position;
                activeCourse.Mark = courseMark;
            }
            activeCourse.MarkSide = MarkSide;
            activeCourse.Order = Order;
            return activeCourse;
        }
    }
}