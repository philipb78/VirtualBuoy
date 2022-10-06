using System;
using System.Collections.Generic;
using System.Text;

namespace Models.CourseItems
{
    public class CourseMark : DBModel
    {
        public virtual Point Position { get; set; }

        public string Name { get; set; }

        public CourseMark()
        {
            Position = new Point();
        }
    }
}