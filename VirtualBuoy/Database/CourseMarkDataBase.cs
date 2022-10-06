using Models.CourseItems;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Database
{
    public class CourseMarkDataBase
    {
        public List<CourseMark> GetCourseMarks()
        {
            Debug.WriteLine("Database Start");
            using (VirtualBuoyDataContext dataContext = new VirtualBuoyDataContext())
            {
                return dataContext.CourseMarks.ToList();
            }
        }

        public void UpdateCourseMark(CourseMark courseMark)
        {
            using (VirtualBuoyDataContext dataContext = new VirtualBuoyDataContext())
            {
                dataContext.Update(courseMark);
                dataContext.SaveChanges();
            }
        }
    }
}